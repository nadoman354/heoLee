using enums;
using UnityEngine;

public class DroppedFactoryHub : MonoBehaviour, IDroppedFactoryHub
{
    public static DroppedFactoryHub I { get; private set; }

    [SerializeField] private DroppedWeaponFactory weaponFactory;
    [SerializeField] private DroppedRelicFactory relicFactory;
    [SerializeField] private DroppedConsumableFactory consumableFactory;
    [SerializeField] private DroppedCurrencyFactory currencyFactory;
    [SerializeField] private DroppedFieldItemFactory fieldItemFactory;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
    }

    public DroppedItem SpawnFromId(ItemType t, string id, Vector3 spawnPos)
    => t switch
    {
        ItemType.Weapon => weaponFactory.CreateFromId(id, spawnPos),
        ItemType.Relic => relicFactory.CreateFromId(id, spawnPos),
        ItemType.Consumable => consumableFactory.CreateFromId(id, spawnPos),
        ItemType.Currency => currencyFactory.CreateFromId(id, spawnPos),
        ItemType.FieldItem => fieldItemFactory.CreateFromId(id, spawnPos),
        _ => null
    };

    public DroppedItem SpawnFromWeapon(IWeaponLogic w, Vector3 spawnPos)
        => weaponFactory.CreateFromWeapon(w, spawnPos);

    public DroppedItem SpawnFromRelic(BaseRelic r, Vector3 spawnPos)
        => relicFactory.CreateFromRelic(r, spawnPos);
}
