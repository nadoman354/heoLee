/*
 * [EnemyBase.cs]
 * - 잡몹 공통 베이스: StatBlock + Health + 간단 FSM
 * - 그로기 미사용(해당 로직 없음)
 * - 이동/어그로/공격 범위/쿨다운 등 기본 필드 포함
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

    // 런타임
    protected StatBlock stats;
    protected Health health;
    protected IEnemyState current;
    protected Transform target;

    public float AttackTimer { get; set; }

    // 프로퍼티(상태에서 접근)
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

    // ── FSM ─────────────────────────────────────────────────
    public void ChangeState(IEnemyState next)
    {
        current?.Exit(this);
        current = next;
        current?.Enter(this);
    }

    // ── IDamageable ─────────────────────────────────────────
    public virtual void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        health.Damage(amount);

        // 피격 시 어그로 유도(선택)
        if (target == null)
        {
            var player = GameObject.FindWithTag("Player");
            target = player ? player.transform : null;
        }
    }

    // ── Hooks ───────────────────────────────────────────────
    protected virtual void OnDeath()
    {
        // 드랍/사운드/이펙트/풀링 반환 등
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // 시각화(디버그)
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
