using UnityEngine;

public class DroppedRelic : DroppedItem
{
    public SO_RelicMetaData meta;
    public Relic original;
    [SerializeField] private RelicFactory relicFactory;

    public override void Setup(ScriptableObject data)
    {
        meta = (SO_RelicMetaData)data;
        original = null;
        SetSprite(meta ? meta.sprite : null);
    }

    public void SetupOriginal(Relic r)
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

        var v = original ?? relicFactory.Create(meta);
        if (inv.TryAddRelic(v)) PoolManager.Despawn(gameObject);
    }
}
