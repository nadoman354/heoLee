/*
 Health.cs  -  단순하고 안전한 HP 시스템 (MonoBehaviour 아님)

 [사용 전 체크리스트]
 1) 반드시 Init(...)를 호출해 MaxHp 키와 선택적 Regen 키, 정책을 전달하세요.
    - 적:    Init(stats, Stats.BasicEnemyStat.MaxHp, Stats.BasicEnemyStat.HealthRegenPerSec, MaxChangePolicy.Clamp);
    - 보스:  Init(stats, Stats.BasicBossStat.MaxHp, null, MaxChangePolicy.Clamp);
    - 플레이어: Init(stats, Stats.BasicPlayerStat.MaxHp, Stats.BasicPlayerStat.HealthRegenPerSec, MaxChangePolicy.KeepRatio);

 2) 수명 주기
    - Awake 또는 Start: health = new Health(); health.Init(...); health.OnDeath += OnDeath;
    - Update: health.Tick();  // 재생이 필요할 때만 호출
    - OnDestroy 등: health.Dispose();  // StatBlock 이벤트 해제

 3) 데미지와 회복
    - 피해: health.Damage(value);  // 0 미만으로 내려가면 0으로 클램프, OnDeath는 한번만 호출
    - 회복: health.Heal(value);    // Max를 넘지 않음. allowOverheal=true로 상한 해제 가능

 4) 스탯 변경 대응
    - StatBlock.OnStatChanged(string key)를 구독한다고 가정
    - MaxHp가 변하면 정책으로 Current를 보정
      Clamp: 현재 HP를 새 Max로 상한 클램프
      KeepRatio: 현재/옛Max 비율을 유지
      KeepAbsolute: 현재 HP 유지, 새 Max만 반영하되 넘치면 클램프

 5) 센티널 값 처리
    - 프로젝트에서 스탯 없음의 센티널을 -999로 사용한다면, GetStatSafe가 fallback으로 대체

 6) 주의
    - 이 타입은 MonoBehaviour가 아님. SerializeField는 동작하지 않음. 반드시 Init(...) 파라미터로 설정
*/

using UnityEngine;

public enum MaxChangePolicy { Clamp, KeepRatio, KeepAbsolute }
/*
 MaxChangePolicy.Clamp 주석용 간단 설명

 한 줄 요약
 - Max가 바뀔 때 현재 HP는 그대로 두고, 새 Max를 넘으면 새 Max로만 잘라 맞춘다.

 규칙
 - Current = min(Current, NewMax)

 예시
 - 현재 80, 새 Max 60  => 결과 60  (넘친 만큼 잘림)
 - 현재 40, 새 Max 60  => 결과 40  (그대로)
 - 현재 30, 새 Max 100 => 결과 30  (그대로)

 언제 쓰나
 - 몬스터, 보스처럼 Max가 바뀌어도 체력 비율까지 바꾸고 싶지 않을 때

 참고: 다른 정책
 - KeepRatio   : 비율 유지. 예) 80/100, 새 Max 60 => 48
 - KeepAbsolute: 현재 구현상 Clamp와 동일. 넘치면 새 Max로만 잘림
*/


public sealed class Health
{
    // Init으로 주입되는 설정값
    string _maxHpKey;                 // MaxHp를 읽을 StatBlock 키
    string _regenKey;                 // 초당 회복을 읽을 키. null이면 미사용
    MaxChangePolicy _policy = MaxChangePolicy.Clamp;

    // 이벤트
    public event System.Action<float> OnDamaged;   // 실제로 깎인 양만 전달
    public event System.Action<float> OnHealed;    // 실제로 회복된 양만 전달
    public event System.Action OnDeath;            // Current가 0이 되는 시점에 한번만 호출

    // 상태
    public float Current { get; private set; }
    public float Max => GetStatSafe(_maxHpKey, 1f);
    public bool IsDead { get; private set; }

    StatBlock _stats;
    float _lastMax;                   // Max 변경 전 값 추적

    // 초기화. 반드시 호출 필요
    public void Init(StatBlock stats, string maxHpKey, string regenKey = null, MaxChangePolicy policy = MaxChangePolicy.Clamp)
    {
        _stats = stats;
        _maxHpKey = maxHpKey;
        _regenKey = regenKey;         // null이면 Tick에서 회복 계산을 건너뜀
        _policy = policy;

        _lastMax = GetStatSafe(_maxHpKey, 1f);
        Current = _lastMax;           // 스폰 시 풀피 시작
        IsDead = false;

        if (_stats != null) _stats.OnStatChanged += OnStatChanged;
    }

    // 이벤트 구독 해제
    public void Dispose()
    {
        if (_stats != null) _stats.OnStatChanged -= OnStatChanged;
        _stats = null;
    }

    // 피해 적용. 0 미만 입력은 무시. 0으로 내려가면 OnDeath 한번만 호출
    public void Damage(float raw)
    {
        if (IsDead) return;

        float before = Current;
        float amount = Mathf.Max(0f, raw);
        Current = Mathf.Max(0f, before - amount);

        float dealt = before - Current;
        if (dealt > 0f) OnDamaged?.Invoke(dealt);

        if (Current <= 0f && !IsDead)
        {
            Current = 0f;
            IsDead = true;
            OnDeath?.Invoke();
        }
    }

    // 회복. 기본은 Max까지. allowOverheal=true면 상한 해제
    public void Heal(float amount, bool allowOverheal = false)
    {
        if (amount <= 0f) return;

        float before = Current;
        float cap = allowOverheal ? float.PositiveInfinity : Max;
        Current = Mathf.Min(cap, before + amount);

        float healed = Current - before;
        if (healed > 0f) OnHealed?.Invoke(healed);
    }

    // StatBlock의 MaxHp 키가 변했을 때 호출
    void OnStatChanged(string key)
    {
        if (string.IsNullOrEmpty(key) || key != _maxHpKey) return;

        float newMax = GetStatSafe(_maxHpKey, 1f);
        float oldMax = _lastMax;
        _lastMax = newMax;

        if (oldMax <= 0f)
        {
            Current = newMax;
            return;
        }

        // Max 변경 정책 적용
        switch (_policy)
        {
            case MaxChangePolicy.KeepRatio:
                {
                    float ratio = Mathf.Clamp01(Current / oldMax);
                    Current = Mathf.Clamp(ratio * newMax, 0f, newMax);
                    break;
                }
            case MaxChangePolicy.KeepAbsolute:
            case MaxChangePolicy.Clamp:
            default:
                Current = Mathf.Min(Current, newMax);
                break;
        }

        // Max 변화로 0 이하가 되었을 때도 사망 처리는 한번만
        if (Current <= 0f && !IsDead)
        {
            Current = 0f;
            IsDead = true;
            OnDeath?.Invoke();
        }
    }

    // 매 프레임 호출 선택. 재생 키가 있을 때만 회복
    public void Tick()
    {
        if (IsDead) return;

        float regenPerSec = GetStatSafe(_regenKey, 0f);
        if (regenPerSec > 0f && Current > 0f && Current < Max)
            Heal(regenPerSec * Time.deltaTime);
    }

    // 내부 유틸. 스탯 없음 센티널(-999)을 fallback으로 대체
    float GetStatSafe(string key, float fallback)
    {
        if (_stats == null || string.IsNullOrEmpty(key)) return fallback;
        float v = _stats.GetStat(key);
        if (Mathf.Approximately(v, -999f)) return fallback;
        return v;
    }
}
