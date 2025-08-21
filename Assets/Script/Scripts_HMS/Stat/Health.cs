using UnityEngine;

public enum MaxChangePolicy { Clamp, KeepRatio, KeepAbsolute }

public sealed class Health
{
    [SerializeField] string maxHpKey = Stats.BasicPlayerStat.MaxHp;
    [SerializeField] MaxChangePolicy onMaxChange = MaxChangePolicy.Clamp;

    public event System.Action<float> OnDamaged;
    public event System.Action<float> OnHealed;
    public event System.Action OnDeath;
    public float Current { get; private set; }

    StatBlock _stats;

    public void Init(StatBlock stats)
    {
        _stats = stats;
        Current = Max;                          // 스폰 시 풀로 시작
        _stats.OnStatChanged += OnStatChanged;  // Key별 이벤트가 있다면 연결
    }

    public float Max => _stats.GetStat(maxHpKey);

    public void Damage(float raw)
    {
        if (Current <= 0) return;
        // (저항/방어/가드 등은 별도 DamagePipeline에서 계산 후 값 투입 권장)
        float before = Current;
        Current = Mathf.Max(0, before - raw);
        OnDamaged?.Invoke(before - Current);
        if (Current <= 0) OnDeath?.Invoke();
    }

    public void Heal(float amount, bool allowOverheal = false)
    {
        float before = Current;
        float cap = allowOverheal ? float.PositiveInfinity : Max;
        Current = Mathf.Min(cap, before + amount);
        OnHealed?.Invoke(Current - before);
    }

    void OnStatChanged(string key)
    {
        if (key != maxHpKey) return;
        float oldMax = Mathf.Max(Max, 0.0001f); // 호출 이전 값이 필요하면 캐시로 전달받도록 설계 가능
        // 정책 적용
        switch (onMaxChange)
        {
            case MaxChangePolicy.KeepRatio:
                float ratio = Current / oldMax;
                Current = Mathf.Clamp(ratio * Max, 0, Max);
                break;
            case MaxChangePolicy.KeepAbsolute:
                Current = Mathf.Min(Current, Max);
                break;
            case MaxChangePolicy.Clamp:
            default:
                Current = Mathf.Min(Current, Max);
                break;
        }
    }

    public void Update()
    {
        // 재생(초당 회복) 예: RegenPerSec 키 사용
        float regen = _stats.GetStat("HealthRegenPerSec");
        if (regen != -999)
        {
            if (regen > 0 && Current > 0 && Current < Max)
                Heal((float)regen * Time.deltaTime);
        }
    }
}
