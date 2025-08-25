/*
 * [EnemyBase.cs]
 * - ��� ���� ���̽�: StatBlock + Health + ���� FSM
 * - �׷α� �̻��(�ش� ���� ����)
 * - �̵�/��׷�/���� ����/��ٿ� �� �⺻ �ʵ� ����
 */
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Data")]
    [SerializeField] private SO_EnemyStatData statData;

    [Header("AI/Combat")]
    [SerializeField] private float aggroRange = 6f;
    [SerializeField] private float loseAggroRange = 9f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1.0f;

    // ��Ÿ��
    protected StatBlock stats;
    protected Health health;
    protected IEnemyState current;
    protected Transform target;

    public float AttackTimer { get; set; }

    // ������Ƽ(���¿��� ����)
    public Transform Target => target;
    public float AggroRange => aggroRange;
    public float LoseAggroRange => loseAggroRange;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    public float MoveSpeed => stats.GetStat(Stats.BasicEnemyStat.MoveSpeed);
    public float Damage => stats.GetStat(Stats.BasicEnemyStat.Damage);
    public float HpCurrent => health.Current;
    public float HpMax => health.Max;

    protected virtual void Awake()
    {
        stats = new StatBlock();
        stats.Init(statData);

        health = new Health();
        health.Init(stats, Stats.BasicEnemyStat.MaxHp, Stats.BasicEnemyStat.HealthRegenPerSec, MaxChangePolicy.Clamp);

        health.OnDeath += OnDeath;

        //�׽�Ʈ
        Debug.Log($"[StatCheck] Enemy.MaxHp = {stats.GetStat(Stats.BasicEnemyStat.MaxHp)}");
        Debug.Log($"[StatCheck] Boss.MaxHp  = {stats.GetStat(Stats.BasicBossStat.MaxHp)}");
        Debug.Log($"[StatCheck] statData = {statData?.name}");
    }

    protected virtual void Start()
    {
        var player = GameObject.FindWithTag("Player");
        target = player ? player.transform : null;

        ChangeState(new EnemyIdleState());

        //�׽�Ʈ
        Debug.Log("HP" + HpCurrent);
    }

    protected virtual void Update()
    {
        health.Tick();
        current?.Tick(this);
    }

    // ���� FSM ��������������������������������������������������������������������������������������������������
    public void ChangeState(IEnemyState next)
    {
        current?.Exit(this);
        current = next;
        current?.Enter(this);
    }

    // ���� IDamageable ����������������������������������������������������������������������������������
    public virtual void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        health.Damage(amount);

        // �ǰ� �� ��׷� ����(����)
        if (target == null)
        {
            var player = GameObject.FindWithTag("Player");
            target = player ? player.transform : null;
        }
    }

    // ���� Hooks ����������������������������������������������������������������������������������������������
    protected virtual void OnDeath()
    {
        // ���/����/����Ʈ/Ǯ�� ��ȯ ��
        Debug.Log("[Enemy] OnDeath fired");        // �� ���� �̺�Ʈ ���� ȣ�� ���� Ȯ��
        ChangeState(new EnemyDeadState());  // ���� ���·� ��ȯ
    }

#if UNITY_EDITOR
    // �ð�ȭ(�����)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.gray; Gizmos.DrawWireSphere(transform.position, loseAggroRange);
    }

    public void TakeDamage(int dmg)
    {
        throw new System.NotImplementedException();
    }
#endif

    // EnemyBase.cs (Ŭ���� ���� �ƹ� ��) [�����ڿ�]]]]]]]]]]]]]]]]]]]]]]   
#if UNITY_EDITOR
    [ContextMenu("Debug/Kill Now (Force)")]
    private void __DebugKillNowForce()
    {
        Debug.Log($"[KillNow] before HP={HpCurrent}");

        // 1) ���� ��������� ū ������� ��� ����
        if (HpCurrent > 0f)
            //TakeDamage(HpCurrent + 9999f);   // float �����ε� ���

        // 2) �׷��� DeadState�� �� ���ٸ�(�̹� �׾� �־��ٸ�) ���� ����
        OnDeath();

        Debug.Log($"[KillNow] after  HP={HpCurrent}");
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Debug/Print Health")]
    private void __DebugPrintHealth()
    {
        // Health Ŭ������ IsDead�� �ִٸ� ���� ���(������ �� ���� �ּ�)
        Debug.Log($"[PrintHealth] HP={HpCurrent}/{HpMax}");
    }
#endif
}
