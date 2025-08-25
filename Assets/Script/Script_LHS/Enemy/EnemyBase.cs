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
        health.Init(stats, Stats.BasicEnemyStat.MaxHp, Stats.BasicEnemyStat.HealthRegenPerSec, MaxChangePolicy.Clamp);

        health.OnDeath += OnDeath;

        //테스트
        Debug.Log($"[StatCheck] Enemy.MaxHp = {stats.GetStat(Stats.BasicEnemyStat.MaxHp)}");
        Debug.Log($"[StatCheck] Boss.MaxHp  = {stats.GetStat(Stats.BasicBossStat.MaxHp)}");
        Debug.Log($"[StatCheck] statData = {statData?.name}");
    }

    protected virtual void Start()
    {
        var player = GameObject.FindWithTag("Player");
        target = player ? player.transform : null;

        ChangeState(new EnemyIdleState());

        //테스트
        Debug.Log("HP" + HpCurrent);
    }

    protected virtual void Update()
    {
        health.Tick();
        current?.Tick(this);
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
        Debug.Log("[Enemy] OnDeath fired");        // ★ 죽음 이벤트 실제 호출 여부 확인
        ChangeState(new EnemyDeadState());  // 죽음 상태로 전환
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

    // EnemyBase.cs (클래스 내부 아무 곳) [개발자용]]]]]]]]]]]]]]]]]]]]]]   
#if UNITY_EDITOR
    [ContextMenu("Debug/Kill Now (Force)")]
    private void __DebugKillNowForce()
    {
        Debug.Log($"[KillNow] before HP={HpCurrent}");

        // 1) 아직 살아있으면 큰 대미지로 사망 유도
        if (HpCurrent > 0f)
            //TakeDamage(HpCurrent + 9999f);   // float 오버로드 명시

        // 2) 그래도 DeadState가 안 들어갔다면(이미 죽어 있었다면) 강제 진입
        OnDeath();

        Debug.Log($"[KillNow] after  HP={HpCurrent}");
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Debug/Print Health")]
    private void __DebugPrintHealth()
    {
        // Health 클래스에 IsDead가 있다면 같이 출력(없으면 이 줄은 주석)
        Debug.Log($"[PrintHealth] HP={HpCurrent}/{HpMax}");
    }
#endif
}
