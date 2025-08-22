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
            // ���� �� �Ҹ�
            PoolManager.Despawn(gameObject);
        }
        else
        {
            // ���� �� �������� �Ҹ����� ��å�� ����
            // PoolManager.Despawn(gameObject);
        }
    }
}
