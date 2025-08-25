using Core.Interfaces;
using UnityEngine;

public sealed class EnemyAttackState : IEnemyState
{
    public void Enter(EnemyBase enemy) { }

    public void Tick(EnemyBase enemy)
    {
        Debug.Log("공격");
        if (!enemy.Target) { enemy.ChangeState(new EnemyIdleState()); return; }

        float dist = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        if (dist > enemy.AttackRange)
        {
            enemy.ChangeState(new EnemyChaseState());
            return;
        }

        // 쿨다운 체크
        enemy.AttackTimer -= Time.deltaTime;
        if (enemy.AttackTimer > 0f) { return; }

        enemy.AttackTimer = enemy.AttackCooldown;

        // ★ 변경: 항상 IAttributeDamageable 경유 -> 실패 시 IDamageable로 폴백
        var target = enemy.Target;

        var attr = target.GetComponent<IAttributeDamageable>();                // 속성/게이지/온히트/유물 경로
        if (attr != null)
        {
            var provider = enemy.GetComponent<IAttributeModifierProvider>();    // 몬스터는 대부분 null일 것
            attr.ApplyAttributeDamage(AttributeType.None, enemy.Damage, 0f, provider);

            // TODO: 히트 리액션/사운드/이펙트
            return;
        }

        // 폴백: 속성 경로가 없으면 HP 직통
        var hp = target.GetComponent<IDamageable>();
        if (hp != null)
        {
            hp.TakeDamage(enemy.Damage);
            // TODO: 히트 리액션/사운드/이펙트
        }
    }

    public void Exit(EnemyBase enemy) { }

    public void Update(EnemyBase enemy) { }
}