using Core.Interfaces;

/// <summary>
/// �Ӽ� �������� ������� �� �ִ� ��� ����Ǵ� ���� ������.
/// ������ �� ��η� ��������(���Ӽ��� type=None, gauge=0).
/// </summary>

public interface IAttributeDamageable
{
    void ApplyAttributeDamage(AttributeType type, float baseDamage, float gaugeAmount);
    void ApplyAttributeDamage(AttributeType type, float baseDamage, float gaugeAmount, IAttributeModifierProvider provider);
}