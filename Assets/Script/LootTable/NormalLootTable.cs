using enums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NormalLootEntry
{
    [Range(0f, 1f)] public float probability = 0f;  // 라인 가중치(합 1 권장)
    public ItemType itemType;                       // None = 미드롭
    public List<string> possibleItemIds = new();    // None이면 무시
}

[CreateAssetMenu(menuName = "Game/NormalLootTable")]
public class NormalLootTable : ScriptableObject
{
    [Header("Relic 전용: 스테이지별 등급 확률")]
    public List<StageRelicRarityChance> rarityChances = new();

    [Header("드롭 테이블 라인(디자이너 관리)")]
    public List<NormalLootEntry> entries = new();

#if UNITY_EDITOR
    // === 에디터: 기본 분포로 자동 채우기 ===
    [ContextMenu("자동 초기화(기본 분포)")]
    private void Editor_AutoFill_Default()
    {
        // 필요시 기본 분포 수정 가능
        LootTableEditorUtility.FillNormal(this, noneProb: 0.65f, weaponProb: 0.05f, relicProb: 0.05f, consumProb: 0.25f);
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[NormalLootTable] 기본 분포로 자동 초기화 완료: {name}");
    }

    // === 에디터: 현재 entries 유지하면서 ID만 카탈로그에서 새로 당겨오기 ===
    [ContextMenu("ID만 새로고침(카탈로그)")]
    private void Editor_RefreshIdsOnly()
    {
        var catalog = ResourcesItemMetaCatalog.LoadDefault();
        if (!catalog)
        {
            Debug.LogWarning("[NormalLootTable] ItemMetaCatalog이 없습니다. Resources/Meta/ItemMetaCatalog 생성 필요.");
            return;
        }

        // 라인 유지, 각 라인의 타입에 맞는 ID만 갱신
        foreach (var e in entries)
        {
            if (e.itemType == ItemType.None) continue;
            e.possibleItemIds = catalog.GetIdsByType(e.itemType);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[NormalLootTable] ID만 새로고침 완료: {name}");
    }

    // === 에디터: Relic 등급 확률 기본값 채우기(없을 때만) ===
    [ContextMenu("Relic 등급 확률 기본값 채우기(1~3스테이지)")]
    private void Editor_FillDefaultRelicRarity()
    {
        if (rarityChances == null) rarityChances = new List<StageRelicRarityChance>();

        void Ensure(int stage, float n, float r, float e)
        {
            var row = rarityChances.Find(x => x.stage == stage);
            if (row == null)
            {
                row = new StageRelicRarityChance { stage = stage, rarityChances = new List<RelicRarityChance>() };
                rarityChances.Add(row);
            }
            row.rarityChances.Clear();
            row.rarityChances.Add(new RelicRarityChance { rarity = RarityType.Normal, probability = n });
            row.rarityChances.Add(new RelicRarityChance { rarity = RarityType.Rare, probability = r });
            row.rarityChances.Add(new RelicRarityChance { rarity = RarityType.Epic, probability = e });
        }

        // 샘플 기본값(원하면 수정)
        Ensure(1, 0.80f, 0.18f, 0.02f);
        Ensure(2, 0.60f, 0.30f, 0.10f);
        Ensure(3, 0.50f, 0.35f, 0.15f);

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[NormalLootTable] Relic 등급 확률 기본값 설정 완료: {name}");
    }
#endif
}
