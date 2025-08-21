using UnityEngine;

public class DroppedConsumable : DroppedItem
{
    public SO_ConsumableMetaData meta;

    public override void Setup(ScriptableObject data)
    {
        meta = (SO_ConsumableMetaData)data;
        SetSprite(meta ? meta.sprite : null);
    }

    public override void OnPlayerNearby() { }
    public override void OnPlayerInteract(Player player)
    {
        if (!CanInteract(player)) return;
        var inv = player?.Inventory; if (inv == null) return;
        
        if (inv.TryAddConsumable(meta, out int index)) PoolManager.Despawn(gameObject);
    }
}
