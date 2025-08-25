using Core.Interfaces;
using UnityEngine;

public sealed class EnemyAttackState : IEnemyState
{
    public void Enter(EnemyBase enemy) { }

    public void Tick(EnemyBase enemy)
    {
        Debug.Log("����");
        if (!enemy.Target) { enemy.ChangeState(new EnemyIdleState()); return; }

        float dist = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        if (dist > enemy.AttackRange)
        {
            enemy.ChangeState(new EnemyChaseState());
            return;
        }

        // ��ٿ� üũ
        enemy.AttackTimer -= Time.deltaTime;
        if (enemy.AttackTimer > 0f) { return; }

        enemy.AttackTimer = enemy.AttackCooldown;

        // �� ����: �׻� IAttributeDamageable ���� -> ���� �� IDamageable�� ����
        var target = enemy.Target;

        var attr = target.GetComponent<IAttributeDamageable>();                // �Ӽ�/������/����Ʈ/���� ���
        if (attr != null)
        {
            var provider = enemy.GetComponent<IAttributeModifierProvider>();    // ���ʹ� ��κ� null�� ��
            attr.ApplyAttributeDamage(AttributeType.None, enemy.Damage, 0f, provider);

            // TODO: ��Ʈ ���׼�/����/����Ʈ
            return;
        }

        // ����: �Ӽ� ��ΰ� ������ HP ����
        var hp = target.GetComponent<IDamageable>();
        if (hp != null)
        {
            hp.TakeDamage(enemy.Damage);
            // TODO: ��Ʈ ���׼�/����/����Ʈ
        }
    }

    public void Exit(EnemyBase enemy) { }

    public void Update(EnemyBase enemy) { }
}