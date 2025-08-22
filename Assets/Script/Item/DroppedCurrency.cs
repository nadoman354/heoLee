using UnityEngine;

public class DroppedCurrency : DroppedItem
{
    public SO_CurrencyMetaData meta;

    public override void Setup(ScriptableObject data)
    {
        meta = (SO_CurrencyMetaData)data;
        SetSprite(meta ? meta.sprite : null);
    }

    public override void OnPlayerNearby() { }

    public override void OnPlayerInteract(Player player)
    {
        if (!CanInteract(player) || !meta) return;

        // ? ¿©±â¼­ È¹µæ
        CurrencyBank.I?.Earn(meta.id, meta.amountPerPickup);

        // ÀÌÆåÆ®/»ç¿îµå ¡¦
        PoolManager.Despawn(gameObject);
    }
}
