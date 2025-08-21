using enums;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources 하위의 단일 경로(타입별 1개)만 스캔해 모든 메타데이터를 보유하는 카탈로그.
/// 에디터/런타임 공용. 프로젝트 전체에서 메타 참조는 이 한 곳만 사용한다.
/// </summary>
[CreateAssetMenu(fileName = "ItemMetaCatalog", menuName = "Game/Meta/ItemMetaCatalog")]
public class ResourcesItemMetaCatalog : ScriptableObject, IItemMetaCatalog
{
    [Header("Resources 하위 검색 경로 (상대경로, 타입별 1개)")]
    [SerializeField] private string weaponPath = "WeaponData";
    [SerializeField] private string relicPath = "RelicData";
    [SerializeField] private string consumablePath = "ConsumableData";

    [Header("캐시 (읽기 전용)")]
    [SerializeField] private List<SO_WeaponMetaData> weapons = new();
    [SerializeField] private List<SO_RelicMetaData> relics = new();
    [SerializeField] private List<SO_ConsumableMetaData> consumables = new();

    // 빠른 조회용 맵
    private Dictionary<string, SO_WeaponMetaData> wMap;
    private Dictionary<string, SO_RelicMetaData> rMap;
    private Dictionary<string, SO_ConsumableMetaData> cMap;
    private Dictionary<string, ItemType> typeMap;

    public IReadOnlyList<SO_WeaponMetaData> Weapons => weapons;
    public IReadOnlyList<SO_RelicMetaData> Relics => relics;
    public IReadOnlyList<SO_ConsumableMetaData> Consumables => consumables;

    /// <summary>
    /// 설정된 단일 경로를 스캔해 캐시를 갱신한다.
    /// - 폴더는 각각 1개만 사용 (WeaponData / RelicData / ConsumableData)
    /// - 중복 제거, 널/빈 ID 스킵
    /// </summary>
    public void Refresh()
    {
        weapons.Clear(); relics.Clear(); consumables.Clear();

        LoadAllInto(weaponPath, weapons);
        LoadAllInto(relicPath, relics);
        LoadAllInto(consumablePath, consumables);

        BuildMaps();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        Debug.Log("[ItemMetaCatalog] Refresh completed.");
    }

    public bool TryGetWeapon(string id, out SO_WeaponMetaData data)
    {
        if (!EnsureMaps() || wMap == null) { data = null; return false; }
        return wMap.TryGetValue(id, out data);
    }

    public bool TryGetRelic(string id, out SO_RelicMetaData data)
    {
        if (!EnsureMaps() || rMap == null) { data = null; return false; }
        return rMap.TryGetValue(id, out data);
    }

    public bool TryGetConsumable(string id, out SO_ConsumableMetaData data)
    {
        if (!EnsureMaps() || cMap == null) { data = null; return false; }
        return cMap.TryGetValue(id, out data);
    }

    public List<string> GetIdsByType(ItemType type)
    {
        var list = new List<string>();
        switch (type)
        {
            case ItemType.IWeaponLogic:
                foreach (var x in weapons) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id);
                break;
            case ItemType.BaseRelic:
                foreach (var x in relics) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id);
                break;
            case ItemType.Consumable:
                foreach (var x in consumables) if (!string.IsNullOrEmpty(x.id)) list.Add(x.id);
                break;
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
        wMap = new(); rMap = new(); cMap = new(); typeMap = new();

        foreach (var x in weapons) if (x && !string.IsNullOrEmpty(x.id)) { wMap[x.id] = x; typeMap[x.id] = ItemType.IWeaponLogic; }
        foreach (var x in relics) if (x && !string.IsNullOrEmpty(x.id)) { rMap[x.id] = x; typeMap[x.id] = ItemType.BaseRelic; }
        foreach (var x in consumables) if (x && !string.IsNullOrEmpty(x.id)) { cMap[x.id] = x; typeMap[x.id] = ItemType.Consumable; }
    }

    private bool EnsureMaps()
    {
        if (wMap != null && rMap != null && cMap != null && typeMap != null) return true;
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
        // 에셋은 있으나 아직 비어있으면 자동 리프레시
        if (autoRefreshIfEmpty && (_cached.weapons.Count + _cached.relics.Count + _cached.consumables.Count) == 0)
            _cached.Refresh();
#endif
        return _cached;
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Now")]
    private void _ctxRefresh() => Refresh();
#endif
}
