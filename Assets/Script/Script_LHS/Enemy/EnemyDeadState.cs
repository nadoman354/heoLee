using UnityEngine;

public class DeadState : IEnemyState
{
    public void Enter(EnemyBase enemy)
    {
        Debug.Log("����");
        GameObject.Destroy(enemy.gameObject);
    }

    public void Update(EnemyBase enemy) { }
    public void Exit(EnemyBase enemy) { }
}