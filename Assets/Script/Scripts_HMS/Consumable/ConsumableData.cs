using UnityEngine;

public abstract class ConsumableData : ScriptableObject, IConsumableItem
{
    public string id;
    public Sprite sprite;
    public string displayName;
    public abstract bool Use(Player player);
}