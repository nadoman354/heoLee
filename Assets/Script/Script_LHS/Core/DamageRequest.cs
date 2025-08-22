using Core.Interfaces;
using UnityEngine;

/// <summary>
/// 한 번의 공격을 기술하는 표준 요청 패킷.
/// 무기/유물/버프/장비에서 발생한 모든 공격 정보를 하나로 묶어서 전달.
/// → IDamageable.TakeDamage / IStaggerable.AddStagger 호출 시 인자로 사용.
/// </summary>
public struct DamageRequest
{
    // HP
    public float baseDamage;          // 원본 공격력

    // Groggy (보스만 해당)
    public float groggyPower;         // 그로기 게이지에 줄 수치

    // 속성 / 게이지
    public AttributeType type;        // 공격 속성 (None, Fire, Ice, Lightning …)
    public float gaugeGain;           // 속성 게이지 누적량
    public IAttributeModifierProvider provider;  // 유물/버프 제공자 (선택)
    public bool bypassAttributes;     // true면 속성/게이지 무시 (트루 데미지)

    // 공격자 팀
    public AttackerTeam attackerTeam; // 플레이어/적/중립

    // 크리티컬
    public bool canCrit;              // 크리티컬 적용 여부
    public float critChance;          // 크리 확률 (0~1)
    public float critMultiplier;      // 크리 배율 (≥1)

    // 온히트 슬로우 (플레이어 전용 효과 예시)
    public float onHitSlowAmount;     // 슬로우 강도(예: 0.2 = 20% 감소)
    public float onHitSlowDuration;   // 슬로우 지속 시간(초)

    // 편의 메서드: 트루 데미지 패킷 생성
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
