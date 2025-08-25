using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyBase enemy);    // 상태 진입 시 1번 실행
    void Tick(EnemyBase enemy);   // 매 프레임 호출
    void Exit(EnemyBase enemy);     // 상태 종료 시 1번 실행
}