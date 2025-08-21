using enums;
using System.Collections.Generic;

public interface IItemMetaCatalog
{
    IReadOnlyList<SO_WeaponMetaData> Weapons { get; }
    IReadOnlyList<SO_RelicMetaData> Relics { get; }
    IReadOnlyList<SO_ConsumableMetaData> Consumables { get; }

    bool TryGetWeapon(string id, out SO_WeaponMetaData data);
    bool TryGetRelic(string id, out SO_RelicMetaData data);
    bool TryGetConsumable(string id, out SO_ConsumableMetaData data);

    List<string> GetIdsByType(ItemType type);
    bool TryGetItemType(string id, out ItemType type);
}