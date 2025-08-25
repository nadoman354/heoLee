using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_FieldItem", menuName = "Game/MetaData/FieldItem")]
public abstract class SO_FieldItemMetaData : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public string name;
    [TextArea] public string description;

    /// <summary>�÷��̾�� ��� ȿ�� ����. ���� �� true.</summary>
    public abstract bool Apply(Player player);
}
