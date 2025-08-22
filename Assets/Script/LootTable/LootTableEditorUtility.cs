#if UNITY_EDITOR
using enums;
using System.Collections.Generic;
using UnityEngine;

public static class LootTableEditorUtility
{
    public static List<string> GetValidIdsByType(ItemType type)
    {
        var catalog = ResourcesItemMetaCatalog.LoadDefault();
        if (!catalog) return new List<string>();
        return catalog.GetIdsByType(type);
    }

    public static List<BossLootEntry> GenerateStageEntries()
    {
        var list = new List<BossLootEntry>();
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType)))
        {
            if (t == ItemType.None) continue;
            list.Add(new BossLootEntry
            {
                itemType = t,
                possibleItemIds = GetValidIdsByType(t),
                dropCountChances = new List<DropCountChance> { new() { count = 1, probability = 1f } }
            });
        }
        return list;
    }

    public static void FillNormal(NormalLootTable t,
                                  float noneProb = 0.65f,
                                  float weaponProb = 0.05f,
                                  float relicProb = 0.05f,
                                  float consumProb = 0.25f)
    {
        if (!t) return;
        t.entries = new List<NormalLootEntry>
        {
            new() { itemType = ItemType.None, probability = noneProb },
            new() { itemType = ItemType.Weapon, probability = weaponProb, possibleItemIds = GetValidIdsByType(ItemType.Weapon) },
            new() { itemType = ItemType.Relic, probability = relicProb, possibleItemIds = GetValidIdsByType(ItemType.Relic) },
            new() { itemType = ItemType.Consumable, probability = consumProb, possibleItemIds = GetValidIdsByType(ItemType.Consumable) },
        };
        UnityEditor.EditorUtility.SetDirty(t);
    }
}
#endif
