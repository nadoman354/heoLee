using UnityEngine;

public sealed class EnemyIdleState : IEnemyState
{
    public void Enter(EnemyBase enemy) { /* ��� �ִϸ��̼� �� */ }

    public void Tick(EnemyBase enemy)
    {
        Debug.Log("���");
        if (!enemy.Target) return;
        float dist = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        if (dist <= enemy.AggroRange)
            enemy.ChangeState(new EnemyChaseState());
    }

    public void Exit(EnemyBase enemy) { }

    public void Update(EnemyBase enemy) { }
}
