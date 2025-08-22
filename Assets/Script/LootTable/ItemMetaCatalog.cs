using System.Collections.Generic;
using UnityEngine;
using enums; // ItemType, RarityType 등 네가 쓰는 네임스페이스

/// <summary>
/// Resources 하위 단일 경로(타입별 1개)만 스캔해서 모든 메타데이터를 보유.
/// 프로젝트 전역 메타 참조 단일 창구.
/// </summary>
[CreateAssetMenu(fileName = "ItemMetaCatalog", menuName = "Game/Meta/ItemMetaCatalog")]
public class ResourcesItemMetaCatalog : ScriptableObject, IItemMetaCatalog
{
    [Header("Resources 하위 검색 경로 (상대경로, 타입별 1개)")]
    [SerializeField] private string weaponPath = "WeaponData";
    [SerializeField] private string relicPath = "RelicData";
    [SerializeField] private string consumablePath = "ConsumableData";
    [SerializeField] private string currencyPath = "CurrencyData";
    [SerializeField] private string fieldItemPath = "FieldItemData";

    [Header("캐시 (읽기 전용)")]
    [SerializeField] private List<SO_WeaponMetaData> weapons = new();
    [SerializeField] private List<SO_RelicMetaData> relics = new();
    [SerializeField] private List<SO_ConsumableMetaData> consumables = new();
    [SerializeField] private List<SO_CurrencyMetaData> currencies = new();
    [SerializeField] private List<SO_FieldItemMetaData> fieldItems = new();

    // 빠른 조회용 맵
    private Dictionary<string, SO_WeaponMetaData> wMap;
    private Dictionary<string, SO_RelicMetaData> rMap;
    private Dictionary<string, SO_ConsumableMetaData> cMap;
    private Dictionary<string, SO_CurrencyMetaData> curMap;
    private Dictionary<string, SO_FieldItemMetaData> fiMap;
    private Dictionary<string, ItemType> typeMap;

    public IReadOnlyList<SO_WeaponMetaData> Weapons => weapons;
    public IReadOnlyList<SO_RelicMetaData> Relics => relics;
    public IReadOnlyList<SO_ConsumableMetaData> Consumables => consumables;
    public IReadOnlyList<SO_CurrencyMetaData> Currencies => currencies;
    public IReadOnlyList<SO_FieldItemMetaData> FieldItems => fieldItems;

    private void OnEnable()
    {
        // 리스트는 채워져 있는데 맵이 비었으면 재구축
        if ((weapons.Count + relics.Count + consumables.Count + currencies.Count + fieldItems.Count) > 0 &&
            (wMap == null || rMap == null || cMap == null || curMap == null || fiMap == null || typeMap == null))
        {
            BuildMaps();
        }
    }

    /// <summary>설정된 경로를 스캔해 캐시 갱신 + 맵 생성</summary>
    public void Refresh()
    {
        weapons.Clear(); relics.Clear(); consumables.Clear(); currencies.Clear(); fieldItems.Clear();

        LoadAllInto(weaponPath, weapons);
        LoadAllInto(relicPath, relics);
        LoadAllInto(consumablePath, consumables);
        LoadAllInto(currencyPath, currencies);
        LoadAllInto(fieldItemPath, fieldItems);

        BuildMaps();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        Debug.Log("[ItemMetaCatalog] Refresh completed.");
    }

    // ----- TryGet 계열 (definite assignment 보장) -----
    public bool TryGetWeapon(string id, out SO_WeaponMetaData data)
    {
        data = null;
        if (!EnsureMaps() || wMap == null) return false;
        return wMap.TryGetValue(id, out data);
    }

    public bool TryGetRelic(string id, out SO_RelicMetaData data)
    {
        data = null;
        if (!EnsureMaps() || rMap == null) return false;
        return rMap.TryGetValue(id, out data);
    }

    public bool TryGetConsumable(string id, out SO_ConsumableMetaData data)
    {
        data = null;
        if (!EnsureMaps() || cMap == null) return false;
        return cMap.TryGetValue(id, out data);
    }
    public bool TryGetCurrency(string id, out SO_CurrencyMetaData data)
    {
        data = null;
        if (!EnsureMaps() || curMap == null) return false;
        return curMap.TryGetValue(id, out data);
    }
    public bool TryGetFieldItem(string id, out SO_FieldItemMetaData data)
    {
        data = null;
        if (!EnsureMaps() || fiMap == null) return false;
        return fiMap.TryGetValue(id, out data);
    }

    public List<string> GetIdsByType(ItemType type)
    {
        var list = new List<string>();
        switch (type)
        {
            case ItemType.Weapon: foreach (var x in weapons) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id); break;
            case ItemType.Relic: foreach (var x in relics) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id); break;
            case ItemType.Consumable: foreach (var x in consumables) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id); break;
            case ItemType.Currency: foreach (var x in currencies) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id); break;
            case ItemType.FieldItem: foreach (var x in fieldItems) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id); break;
        }
        return list;
    }

    public bool TryGetItemType(string id, out ItemType type)
    {
        EnsureMaps();
        return typeMap.TryGetValue(id, out type);
    }

    // ---------- 내부 유틸 ----------
    private void LoadAllInto<T>(string path, List<T> dst) where T : ScriptableObject
    {
        if (string.IsNullOrEmpty(path)) return;
        var arr = Resources.LoadAll<T>(path);
        var seen = new HashSet<T>();
        foreach (var so in arr)
        {
            if (!so) continue;
            if (!seen.Add(so)) continue;   // 중복 제거
            dst.Add(so);
        }
    }

    private void BuildMaps()
    {
        wMap = new(); rMap = new(); cMap = new(); curMap = new(); fiMap = new(); typeMap = new();
        var dupCheck = new HashSet<string>();

        void AddType(string id, ItemType t)
        {
            if (!dupCheck.Add(id))
                Debug.LogWarning($"[ItemMetaCatalog] Duplicate id detected: {id} ({t})");
            typeMap[id] = t;
        }

        foreach (var x in weapons) if (x && !string.IsNullOrEmpty(x.id)) { wMap[x.id] = x; AddType(x.id, ItemType.Weapon); }
        foreach (var x in relics) if (x && !string.IsNullOrEmpty(x.id)) { rMap[x.id] = x; AddType(x.id, ItemType.Relic); }
        foreach (var x in consumables) if (x && !string.IsNullOrEmpty(x.id)) { cMap[x.id] = x; AddType(x.id, ItemType.Consumable); }
        foreach (var x in currencies) if (x && !string.IsNullOrEmpty(x.id)) { curMap[x.id] = x; AddType(x.id, ItemType.Currency); }
        foreach (var x in fieldItems) if (x && !string.IsNullOrEmpty(x.id)) { fiMap[x.id] = x; AddType(x.id, ItemType.FieldItem); }
    }

    private bool EnsureMaps()
    {
        if (wMap != null && rMap != null && cMap != null && curMap != null && fiMap != null && typeMap != null) return true;
        BuildMaps();
        return true;
    }

    // ---------- 정적 접근(리소스에서 로드) ----------
    private const string DEFAULT_RES_PATH = "Meta/ItemMetaCatalog"; // Assets/Resources/Meta/ItemMetaCatalog.asset
    private static ResourcesItemMetaCatalog _cached;
    public static ResourcesItemMetaCatalog LoadDefault(bool autoRefreshIfEmpty = true)
    {
        if (!_cached) _cached = Resources.Load<ResourcesItemMetaCatalog>(DEFAULT_RES_PATH);
        if (!_cached)
        {
            Debug.LogWarning($"[ItemMetaCatalog] Not found at Resources/{DEFAULT_RES_PATH}. Create via menu Game/Meta/ItemMetaCatalog");
            return null;
        }
#if UNITY_EDITOR
        if (autoRefreshIfEmpty &&
            (_cached.weapons.Count + _cached.relics.Count + _cached.consumables.Count + _cached.currencies.Count + _cached.fieldItems.Count) == 0)
            _cached.Refresh();
#endif
        return _cached;
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Now")]
    private void _ctxRefresh() => Refresh();
#endif
}
