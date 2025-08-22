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

        // ��ٿ� üũ
        enemy.AttackTimer -= Time.deltaTime;
        if (enemy.AttackTimer <= 0f)
        {
            enemy.AttackTimer = enemy.AttackCooldown;

            // ���� �ܼ��� ���� ���� ����: �÷��̾ ������ ����
            var targetDamageable = enemy.Target.GetComponent<IDamageable>();
            if (targetDamageable != null)
            {
                float dmg = enemy.Damage;
                targetDamageable.TakeDamage(dmg);
                // �� �ڸ��� ��Ʈ ���׼�/����/����Ʈ �� ȣ��
            }
        }
    }

    public void Exit(EnemyBase enemy) { }

    public void Update(EnemyBase enemy) { }
}