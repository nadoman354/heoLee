 public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void ChangeState(IEnemyState newState, EnemyBase enemy)
    {
        currentState?.Exit(enemy);       // ���� ���� ����
        currentState = newState;
        currentState.Enter(enemy);       // �� ���� ����
    }

    public void Update(EnemyBase enemy)
    {
        currentState?.Update(enemy);     // ���� ���� �����Ӹ��� ������Ʈ
    }
}