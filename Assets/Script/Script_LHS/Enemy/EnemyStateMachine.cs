 public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void ChangeState(IEnemyState newState, EnemyBase enemy)
    {
        currentState?.Exit(enemy);       // 이전 상태 종료
        currentState = newState;
        currentState.Enter(enemy);       // 새 상태 진입
    }

    public void Update(EnemyBase enemy)
    {
        currentState?.Update(enemy);     // 현재 상태 프레임마다 업데이트
    }
}