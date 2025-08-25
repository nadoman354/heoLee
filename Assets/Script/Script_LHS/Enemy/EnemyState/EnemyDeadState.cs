using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    public void Enter(EnemyBase enemy)
    {
        Debug.Log("����");
        GameObject.Destroy(enemy.gameObject);
    }

    public void Tick(EnemyBase enemy) {
        Debug.Log("������");
    }

    public void Exit(EnemyBase enemy) { }
}