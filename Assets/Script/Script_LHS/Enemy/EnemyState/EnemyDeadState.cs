using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    public void Enter(EnemyBase enemy)
    {
        Debug.Log("Á×À½");
        GameObject.Destroy(enemy.gameObject);
    }

    public void Tick(EnemyBase enemy) {
        Debug.Log("Á×À½Áß");
    }

    public void Exit(EnemyBase enemy) { }
}