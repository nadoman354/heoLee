using UnityEngine;

public class DroppedWeapon : DroppedItem
{
    public SO_WeaponMetaData meta;
    public IWeaponLogic original;

    public override void Setup(ScriptableObject data)
    {
        meta = (SO_WeaponMetaData)data;
        original = null;
        SetSprite(meta ? meta.sprite : null);
    }

    public void SetupOriginal(IWeaponLogic w)
    {
        original = w;
        meta = w.GetMetaData();
        SetSprite(meta ? meta.sprite : null);
    }

    public override void OnPlayerNearby() { }
    public override void OnPlayerInteract(Player player)
    {
        if (!CanInteract(player)) return;
        ICapabilities capabilities = player.WeaponController.Caps;
        WeaponView view = player.WeaponController.View;
        var w = original ?? LogicFactoryHub.WeaponFactory.Create(meta, view, capabilities);
        var inv = player?.Inventory; if (inv == null) return;
        if (inv.TryAddWeapon(w, meta)) PoolManager.Despawn(gameObject);
    }

    ///<summary>
    /// 여기 아래로는 테. 스. 트. 코드입니다. 나중에 지워야 함
    ///</summary>
    /// 충돌 시 먹어지는 기능
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            OnPlayerInteract(collision.GetComponent<Player>());
        }
    }
}
