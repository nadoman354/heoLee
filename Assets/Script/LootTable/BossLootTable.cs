using enums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossLootEntry
{
    public ItemType itemType;                     // 이 라인의 타입
    public List<string> possibleItemIds = new();  // 후보 ID
    public List<DropCountChance> dropCountChances = new(); // 예: 7(0.5), 8(0.3), 9(0.2)
}

[CreateAssetMenu(menuName = "Game/BossLootTable")]
public class BossLootTable : ScriptableObject
{
    [Header("Relic 등급 확률(스테이지별)")]
    public List<StageRelicRarityChance> rarityChances = new();

    [Header("Stage 1 Loot")]
    public List<BossLootEntry> stage1Loot = new();
    [Header("Stage 2 Loot")]
    public List<BossLootEntry> stage2Loot = new();
    [Header("Stage 3 Loot")]
    public List<BossLootEntry> stage3Loot = new();

#if UNITY_EDITOR
    // === 에디터: 3스테이지 엔트리 자동 생성(가능한 ID 전부 포함) ===
    [ContextMenu("자동 초기화(3 스테이지)")]
    private void Editor_AutoInit_AllStages()
    {
        stage1Loot = LootTableEditorUtility.GenerateStageEntries();
        stage2Loot = LootTableEditorUtility.GenerateStageEntries();
        stage3Loot = LootTableEditorUtility.GenerateStageEntries();

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[BossLootTable] 3 Stage 자동 초기화 완료: {name}");
    }

    // === 에디터: 현재 구조는 유지하면서 ID만 카탈로그 기준으로 싹 갱신 ===
    [ContextMenu("ID만 새로고침(카탈로그)")]
    private void Editor_RefreshIdsOnly_AllStages()
    {
        var catalog = ResourcesItemMetaCatalog.LoadDefault();
        if (!catalog)
        {
            Debug.LogWarning("[BossLootTable] ItemMetaCatalog이 없습니다. Resources/Meta/ItemMetaCatalog 생성 필요.");
            return;
        }

        void RefreshList(List<BossLootEntry> list)
        {
            if (list == null) return;
            foreach (var e in list)
            {
                if (e == null) continue;
                if (e.itemType == ItemType.None) { e.possibleItemIds?.Clear(); continue; }
                e.possibleItemIds = catalog.GetIdsByType(e.itemType);
            }
        }

        RefreshList(stage1Loot);
        RefreshList(stage2Loot);
        RefreshList(stage3Loot);

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[BossLootTable] ID만 새로고침 완료: {name}");
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
        Debug.Log($"[BossLootTable] Relic 등급 확률 기본값 설정 완료: {name}");
    }
#endif
}
