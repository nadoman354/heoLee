using UnityEngine;

public class DroppedRelic : DroppedItem
{
    public SO_RelicMetaData meta;
    public BaseRelic original;

    public override void Setup(ScriptableObject data)
    {
        meta = (SO_RelicMetaData)data;
        original = null;
        SetSprite(meta ? meta.sprite : null);
    }

    public void SetupOriginal(BaseRelic r)
    {
        original = r;
        meta = r.GetMetaData();
        SetSprite(meta ? meta.sprite : null);
    }

    public override void OnPlayerNearby() { }
    public override void OnPlayerInteract(Player player)
    {
        if (!CanInteract(player)) return;
        var inv = player?.Inventory; if (inv == null) return;

        var v = original ?? LogicFactoryHub.RelicFactory.Create(meta, player.Inventory);
        if (inv.TryAddRelic(v)) PoolManager.Despawn(gameObject);
    }
}
