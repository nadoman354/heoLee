using UnityEngine;
using System.Collections.Generic;
using enums;

public interface IRng
{
    System.Random GetScoped(string domain, params int[] tags);
}

public interface IUnlockMask
{
    List<string> FilterEligible(ItemType type, List<string> ids);
}

public interface IDroppedFactoryHub
{
    DroppedItem SpawnFromId(ItemType type, string id, Vector3 spawnPos);
    DroppedItem SpawnFromWeapon(IWeaponLogic w, Vector3 spawnPos);
    DroppedItem SpawnFromRelic(BaseRelic r, Vector3 spawnPos);
}
