/*
 * [BossBase.cs]
 * 무엇: 보스 전투 컨트롤러(HP + Groggy + 패턴 러너).
 *
 * 책임:
 *  - Health/GroggyMeter 초기화 및 상태 관리.
 *  - PatternRunner로 패턴 순환, 그로기 중에는 일시정지.
 *  - DamageRequest를 받아 CombatResolver로 계산 후 HP/그로기 적용.
 *
 * 사용법:
 *  - SO_BossStatData, BossPatternSet 연결.
 *  - 외부(무기/스킬)에서 ApplyHit(req, attackerStats) 호출.
 *  - 패턴 특수 발동이 필요하면 TriggerSpecialPattern 사용.
 */

using Core.Interfaces;
using UnityEngine;

public class BossBase : MonoBehaviour, IDamageable, IStaggerable
{

    [Header("Stats / Data")]
    [SerializeField] private SO_BossStatData bossStatData;
    [SerializeField] private BossPatternSet patternSet;

    private StatBlock stats;
    private Health health;
    private GroggyMeter groggy;
    private PatternRunner runner;

    private Transform player;
    private bool isDead;
    private float groggyTimer;

    public bool IsGroggy => groggy?.IsGroggy ?? false;
    public float HealthPercent => Mathf.Approximately(health.Max, 0f) ? 0f : (health.Current / health.Max);
    public Transform Player => player;

    void Awake()
    {
        stats = new StatBlock(); stats.Init(bossStatData);
        health = new Health(); health.Init(stats);
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;

        float maxG = stats.GetStat(Stats.BasicBossStat.GroggyMax);
        float dur = CombatResolver.ComputeGroggyDuration(stats); // 보스 스탯 기반 기본 지속 계산
        //groggy = new GroggyMeter(maxG, dur, decayPerSec: 0f, lockoutAfter: 1.2f); 
        groggy.SetDurationFinal(dur);

        groggy.OnEnter += () => { runner?.Pause(); groggyTimer = 0f; Debug.Log("&#x1f4ab; Groggy!"); };
        groggy.OnExit += () => { Debug.Log("&#x1f9e0; Groggy End"); };

        health.OnDeath += () =>
        {
            isDead = true;
            runner?.Pause();
            Destroy(gameObject);
        };

        runner = new PatternRunner(this, patternSet);

        // 지속시간 관련 스탯 변화 반영(선택)
        stats.OnStatChanged += key =>
        {
            if (key == Stats.BasicBossStat.GroggyBaseDuration ||
                key == Stats.GroggyKeys.DurationAdd ||
                key == Stats.GroggyKeys.DurationMulSum)
            {
                groggy.SetDurationFinal(CombatResolver.ComputeGroggyDuration(stats));
            }
        };
    }

    void Update()
    {
        if (isDead) return;

        health.Tick();
        groggy.Tick(Time.deltaTime);

        if (IsGroggy)
        {
            groggyTimer += Time.deltaTime;
            if (groggyTimer >= groggy.Duration)
            {
                groggyTimer = 0f;
                groggy.Exit();
            }
            return; // 다운 동안 패턴 정지
        }

        runner.Tick();
    }

    // ── IDamageable ──────────────────────────────
    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        health.Damage(dmg);
    }

    // ── IStaggerable ─────────────────────────────
    public void AddStagger(float amount)
    {
        if (isDead || IsGroggy) return;
        groggy.Add(amount);
    }

    // 패턴 종료 콜백(러너가 FinishPattern 시 호출)
    public void OnPatternFinished() { /* 러너가 다음 선택 처리 */ }

    public void TriggerSpecialPattern(string trigger) => runner?.ForceSpecial(trigger);

    /// <summary>한 번의 공격 적용: 공격자 스탯(유물 보정 포함) 전달 필수</summary>
    public void ApplyHit(in DamageRequest req, StatBlock attackerStats)
    {
        if (isDead) return;

        var result = CombatResolver.Resolve(in req, attackerStats);

        if (result.finalHpDamage > 0f) TakeDamage(result.finalHpDamage);
        if (result.finalGroggyAdd > 0f) AddStagger(result.finalGroggyAdd);

        if (req.attackerTeam == AttackerTeam.Player && req.onHitSlowAmount > 0f)
        {
            // TODO: 슬로우/디버프 시스템 연동
            // ApplySlow(req.onHitSlowAmount, req.onHitSlowDuration);
        }

        // 속성/게이지 파이프라인은 별도 시스템에서 처리(여기서는 수치만 결정 가능)
    }
}
