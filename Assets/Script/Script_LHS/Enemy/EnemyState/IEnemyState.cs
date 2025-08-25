using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyBase enemy);    // ���� ���� �� 1�� ����
    void Tick(EnemyBase enemy);   // �� ������ ȣ��
    void Exit(EnemyBase enemy);     // ���� ���� �� 1�� ����
}