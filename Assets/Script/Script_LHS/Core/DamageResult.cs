/*
 * [CombatResolver.cs]
 * ����: DamageRequest(�Է�) �� DamageResult(���) ����.
 *  - HP: ũ��Ƽ�� �ݿ�
 *  - �׷α�: (���� groggyPower + ��(+)) �� (1 + ��(��%)) ��Ģ
 *  - ComputeGroggyDuration ��ƿ ����
 *
 * å��:
 *  - �뷱�� ������ ���� ������, ��Ģ �߾�����ȭ.
 *
 * ����:
 *  - var result = CombatResolver.Resolve(req, attackerStats);
 *  - result.finalHpDamage / finalGroggyAdd�� ��� ����.
 *  - ���� �׷α� ���ӽð� ������ ComputeGroggyDuration ���.
 */

using UnityEngine;

public struct DamageResult
{
    public float finalHpDamage;
    public float finalGroggyAdd;
    public bool  isCritical;
}