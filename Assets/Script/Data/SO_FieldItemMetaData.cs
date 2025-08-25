using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_FieldItem", menuName = "Game/MetaData/FieldItem")]
public abstract class SO_FieldItemMetaData : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public string name;
    [TextArea] public string description;

    /// <summary>플레이어에게 즉시 효과 적용. 성공 시 true.</summary>
    public abstract bool Apply(Player player);
}
