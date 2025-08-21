using enums;
using System.Collections.Generic;
using UnityEngine;

/// <summary>플레이어 런타임 해금 마스크. SO에는 상태 저장 X.</summary>
public class UnlockService : MonoBehaviour, IUnlockMask
{
    public static UnlockService I { get; private set; }

    [SerializeField] private ResourcesItemMetaCatalog metaCatalog;

    private readonly HashSet<string> unlocked = new(); // "ItemTypeName:id"
    private Dictionary<string, ItemType> idToType;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
        if (!metaCatalog) metaCatalog = ResourcesItemMetaCatalog.LoadDefault();
    }

    private static string Key(ItemType t, string id) => $"{t}:{id}";

    public bool IsUnlocked(ItemType type, string id) => unlocked.Contains(Key(type, id));

    public List<string> FilterEligible(ItemType type, List<string> ids)
    {
        var res = new List<string>(ids?.Count ?? 0);
        if (ids == null) return res;
        foreach (var id in ids) if (!string.IsNullOrEmpty(id) && IsUnlocked(type, id)) res.Add(id);
        return res;
    }

    public int UnlockByIds(ItemType type, IEnumerable<string> ids)
    {
        if (ids == null) return 0;
        int added = 0;
        foreach (var id in ids) if (!string.IsNullOrEmpty(id) && unlocked.Add(Key(type, id))) added++;
        return added;
    }

    public int UnlockByIdsAuto(IEnumerable<string> ids)
    {
        if (ids == null) return 0;
        EnsureIdCatalog();
        int added = 0;
        foreach (var id in ids)
        {
            if (string.IsNullOrEmpty(id)) continue;
            if (!idToType.TryGetValue(id, out var type)) continue;
            if (unlocked.Add(Key(type, id))) added++;
        }
        return added;
    }

    private void EnsureIdCatalog()
    {
        if (idToType != null) return;
        idToType = new Dictionary<string, ItemType>(512);
        if (!metaCatalog) return;

        foreach (var w in metaCatalog.Weapons) if (w && !string.IsNullOrEmpty(w.id)) idToType[w.id] = ItemType.Weapon;
        foreach (var r in metaCatalog.Relics) if (r && !string.IsNullOrEmpty(r.id)) idToType[r.id] = ItemType.Relic;
        foreach (var c in metaCatalog.Consumables) if (c && !string.IsNullOrEmpty(c.id)) idToType[c.id] = ItemType.Consumable;
    }
}
