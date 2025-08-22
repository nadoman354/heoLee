using UnityEngine;

public class DroppedFieldItem : DroppedItem
{
    public SO_FieldItemMetaData meta;

    public override void Setup(ScriptableObject data)
    {
        meta = (SO_FieldItemMetaData)data;
        SetSprite(meta ? meta.sprite : null);
    }

    public override void OnPlayerNearby() { }

    public override void OnPlayerInteract(Player player)
    {
        if (!CanInteract(player)) return;
        if (!meta || !meta.effect) return;

        if (meta.effect.Apply(player))
        {
            // 성공 시 소멸
            PoolManager.Despawn(gameObject);
        }
        else
        {
            // 실패 시 유지할지 소멸할지 정책에 따라
            // PoolManager.Despawn(gameObject);
        }
    }
}
