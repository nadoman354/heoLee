using UnityEngine;

public sealed class EnemyAttackState : IEnemyState
{
    public void Enter(EnemyBase enemy) { }

    public void Tick(EnemyBase enemy)
    {
        if (!enemy.Target) { enemy.ChangeState(new EnemyIdleState()); return; }

        float dist = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        if (dist > enemy.AttackRange)
        {
            enemy.ChangeState(new EnemyChaseState());
            return;
        }

        // 쿨다운 체크
        enemy.AttackTimer -= Time.deltaTime;
        if (enemy.AttackTimer <= 0f)
        {
            enemy.AttackTimer = enemy.AttackCooldown;

            // 아주 단순한 근접 공격 예시: 플레이어에 데미지 적용
            var targetDamageable = enemy.Target.GetComponent<IDamageable>();
            if (targetDamageable != null)
            {
                float dmg = enemy.Damage;
                targetDamageable.TakeDamage(dmg);
                // 이 자리에 히트 리액션/사운드/이펙트 등 호출
            }
        }
    }

    public void Exit(EnemyBase enemy) { }

    public void Update(EnemyBase enemy) { }
}