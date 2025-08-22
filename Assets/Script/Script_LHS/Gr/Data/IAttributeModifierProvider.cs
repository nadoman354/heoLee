using Core.Interfaces;

/// <summary>
/// ������(�÷��̾�/����/���� ��)�� ����.
/// ��û�� �Ӽ�(type)�� ���� ������ ����ġ(AttributeMods)�� ����.
/// </summary>

public interface IAttributeModifierProvider
{
    AttributeMods GetAttributeMods(AttributeType type);
}