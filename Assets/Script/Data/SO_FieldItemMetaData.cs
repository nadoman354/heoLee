using UnityEngine;

[CreateAssetMenu(fileName = "MetaData_FieldItem", menuName = "Game/MetaData/FieldItem")]
public class SO_FieldItemMetaData : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public string name;
    [TextArea] public string description;

    public FieldEffectBase effect; // 즉시 발동 효과
}

/// <summary>필드 아이템 효과 베이스. 구체 효과는 이걸 상속한 SO로 구현</summary>
public abstract class FieldEffectBase : ScriptableObject
{
    /// <returns>적용 성공 여부</returns>
    public abstract bool Apply(Player player);
}
