using UnityEngine;

public abstract class SO_ConsumableMetaData : ScriptableObject, IConsumableItem
{
    public string id;
    public Sprite sprite;
    public string displayName;
    public abstract bool Use(Player player);
}