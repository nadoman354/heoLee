using Core.Interfaces;
using UnityEngine;

/// <summary>
/// �� ���� ������ ����ϴ� ǥ�� ��û ��Ŷ.
/// ����/����/����/��񿡼� �߻��� ��� ���� ������ �ϳ��� ��� ����.
/// �� IDamageable.TakeDamage / IStaggerable.AddStagger ȣ�� �� ���ڷ� ���.
/// </summary>
public struct DamageRequest
{
    // HP
    public float baseDamage;          // ���� ���ݷ�

    // Groggy (������ �ش�)
    public float groggyPower;         // �׷α� �������� �� ��ġ

    // �Ӽ� / ������
    public AttributeType type;        // ���� �Ӽ� (None, Fire, Ice, Lightning ��)
    public float gaugeGain;           // �Ӽ� ������ ������
    public IAttributeModifierProvider provider;  // ����/���� ������ (����)
    public bool bypassAttributes;     // true�� �Ӽ�/������ ���� (Ʈ�� ������)

    // ������ ��
    public AttackerTeam attackerTeam; // �÷��̾�/��/�߸�

    // ũ��Ƽ��
    public bool canCrit;              // ũ��Ƽ�� ���� ����
    public float critChance;          // ũ�� Ȯ�� (0~1)
    public float critMultiplier;      // ũ�� ���� (��1)

    // ����Ʈ ���ο� (�÷��̾� ���� ȿ�� ����)
    public float onHitSlowAmount;     // ���ο� ����(��: 0.2 = 20% ����)
    public float onHitSlowDuration;   // ���ο� ���� �ð�(��)

    // ���� �޼���: Ʈ�� ������ ��Ŷ ����
    public static DamageRequest TrueDamage(float dmg) => new DamageRequest
    {
        baseDamage = dmg,
        groggyPower = 0f,
        type = AttributeType.None,
        gaugeGain = 0f,
        provider = null,
        bypassAttributes = true,
        attackerTeam = AttackerTeam.Neutral,
        canCrit = false,
        critChance = 0f,
        critMultiplier = 1f,
        onHitSlowAmount = 0f,
        onHitSlowDuration = 0f
    };
}
