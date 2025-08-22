using UnityEngine;


public sealed class EnemyChaseState : IEnemyState
{
    public void Enter(EnemyBase enemy) { }

    public void Tick(EnemyBase enemy)
    {
        if (!enemy.Target) { enemy.ChangeState(new EnemyIdleState()); return; }

        Vector3 to = (enemy.Target.position - enemy.transform.position).normalized;
        float spd = enemy.MoveSpeed;
        enemy.transform.position += to * spd * Time.deltaTime;

        float dist = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        if (dist <= enemy.AttackRange)
            enemy.ChangeState(new EnemyAttackState());
        else if (dist > enemy.LoseAggroRange)
            enemy.ChangeState(new EnemyIdleState());
    }

    public void Exit(EnemyBase enemy) { }

    public void Update(EnemyBase enemy) { }
}
