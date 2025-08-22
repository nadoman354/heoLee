/*
 * [BossBase.cs]
 * ����: ���� ���� ��Ʈ�ѷ�(HP + Groggy + ���� ����).
 *
 * å��:
 *  - Health/GroggyMeter �ʱ�ȭ �� ���� ����.
 *  - PatternRunner�� ���� ��ȯ, �׷α� �߿��� �Ͻ�����.
 *  - DamageRequest�� �޾� CombatResolver�� ��� �� HP/�׷α� ����.
 *
 * ����:
 *  - SO_BossStatData, BossPatternSet ����.
 *  - �ܺ�(����/��ų)���� ApplyHit(req, attackerStats) ȣ��.
 *  - ���� Ư�� �ߵ��� �ʿ��ϸ� TriggerSpecialPattern ���.
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
        float dur = CombatResolver.ComputeGroggyDuration(stats); // ���� ���� ��� �⺻ ���� ���
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

        // ���ӽð� ���� ���� ��ȭ �ݿ�(����)
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
            return; // �ٿ� ���� ���� ����
        }

        runner.Tick();
    }

    // ���� IDamageable ������������������������������������������������������������
    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        health.Damage(dmg);
    }

    // ���� IStaggerable ����������������������������������������������������������
    public void AddStagger(float amount)
    {
        if (isDead || IsGroggy) return;
        groggy.Add(amount);
    }

    // ���� ���� �ݹ�(���ʰ� FinishPattern �� ȣ��)
    public void OnPatternFinished() { /* ���ʰ� ���� ���� ó�� */ }

    public void TriggerSpecialPattern(string trigger) => runner?.ForceSpecial(trigger);

    /// <summary>�� ���� ���� ����: ������ ����(���� ���� ����) ���� �ʼ�</summary>
    public void ApplyHit(in DamageRequest req, StatBlock attackerStats)
    {
        if (isDead) return;

        var result = CombatResolver.Resolve(in req, attackerStats);

        if (result.finalHpDamage > 0f) TakeDamage(result.finalHpDamage);
        if (result.finalGroggyAdd > 0f) AddStagger(result.finalGroggyAdd);

        if (req.attackerTeam == AttackerTeam.Player && req.onHitSlowAmount > 0f)
        {
            // TODO: ���ο�/����� �ý��� ����
            // ApplySlow(req.onHitSlowAmount, req.onHitSlowDuration);
        }

        // �Ӽ�/������ ������������ ���� �ý��ۿ��� ó��(���⼭�� ��ġ�� ���� ����)
    }
}
