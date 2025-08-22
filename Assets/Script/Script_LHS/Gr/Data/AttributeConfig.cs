using Core.Interfaces;
using UnityEngine;

/// <summary>
/// 속성별 기본 설정(인스펙터 조정): 발현까지의 MaxGauge, 발현 배율, 효과 키.
/// </summary>
[System.Serializable]
public class AttributeConfig
{
    public AttributeType type;
    [Min(1f)] public float maxGauge = 100f;
    [Min(0f)] public float activatedDamageMultiplier = 3f;
    public string effectName; // StatusEffectManager에서 라우팅할 키(선택)
}
