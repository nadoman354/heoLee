using Core.Interfaces;

/// <summary>
/// 속성 데미지를 적용받을 수 있는 대상에 노출되는 단일 진입점.
/// 무조건 이 경로로 때리세요(무속성은 type=None, gauge=0).
/// </summary>

public interface IAttributeDamageable
{
    void ApplyAttributeDamage(AttributeType type, float baseDamage, float gaugeAmount);
    void ApplyAttributeDamage(AttributeType type, float baseDamage, float gaugeAmount, IAttributeModifierProvider provider);
}