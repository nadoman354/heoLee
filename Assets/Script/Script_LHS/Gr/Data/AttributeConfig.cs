using Core.Interfaces;
using UnityEngine;

/// <summary>
/// �Ӽ��� �⺻ ����(�ν����� ����): ���������� MaxGauge, ���� ����, ȿ�� Ű.
/// </summary>
[System.Serializable]
public class AttributeConfig
{
    public AttributeType type;
    [Min(1f)] public float maxGauge = 100f;
    [Min(0f)] public float activatedDamageMultiplier = 3f;
    public string effectName; // StatusEffectManager���� ������� Ű(����)
}
