using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_FieldItem", menuName = "Game/MetaData/FieldItem")]
public class SO_FieldItemMetaData : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public string name;
    [TextArea] public string description;

    public FieldEffectBase effect; // ��� �ߵ� ȿ��
}

/// <summary>�ʵ� ������ ȿ�� ���̽�. ��ü ȿ���� �̰� ����� SO�� ����</summary>
public abstract class FieldEffectBase : ScriptableObject
{
    /// <returns>���� ���� ����</returns>
    public abstract bool Apply(Player player);
}
