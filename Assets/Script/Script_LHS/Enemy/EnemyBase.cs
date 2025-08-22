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
        health.Init(stats);

        health.OnDeath += OnDeath;
    }

    protected virtual void Start()
    {
        var player = GameObject.FindWithTag("Player");
        target = player ? player.transform : null;

        ChangeState(new EnemyIdleState());
    }

    protected virtual void Update()
    {
        health.Tick();
        current?.Update(this);
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
        Destroy(gameObject);
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
}
