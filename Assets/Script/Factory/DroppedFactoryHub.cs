using enums;
using UnityEngine;

public class DroppedFactoryHub : MonoBehaviour, IDroppedFactoryHub
{
    public static DroppedFactoryHub I { get; private set; }

    [SerializeField] private DroppedWeaponFactory weaponFactory;
    [SerializeField] private DroppedRelicFactory relicFactory;
    [SerializeField] private DroppedConsumableFactory consumableFactory;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
    }

    public DroppedItem SpawnFromId(ItemType t, string id, Vector3 spawnPos)
        => t switch
        {
            ItemType.IWeaponLogic => weaponFactory.CreateFromId(id, spawnPos),
            ItemType.BaseRelic => relicFactory.CreateFromId(id, spawnPos),
            ItemType.Consumable => consumableFactory.CreateFromId(id, spawnPos),
            _ => null
        };

    public DroppedItem SpawnFromWeapon(IWeaponLogic w, Vector3 spawnPos)
        => weaponFactory.CreateFromWeapon(w, spawnPos);

    public DroppedItem SpawnFromRelic(BaseRelic r, Vector3 spawnPos)
        => relicFactory.CreateFromRelic(r, spawnPos);
}
