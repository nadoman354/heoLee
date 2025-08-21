using enums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossLootEntry
{
    public ItemType itemType;                     // �� ������ Ÿ��
    public List<string> possibleItemIds = new();  // �ĺ� ID
    public List<DropCountChance> dropCountChances = new(); // ��: 7(0.5), 8(0.3), 9(0.2)
}

[CreateAssetMenu(menuName = "Game/BossLootTable")]
public class BossLootTable : ScriptableObject
{
    [Header("Relic ��� Ȯ��(����������)")]
    public List<StageRelicRarityChance> rarityChances = new();

    [Header("Stage 1 Loot")]
    public List<BossLootEntry> stage1Loot = new();
    [Header("Stage 2 Loot")]
    public List<BossLootEntry> stage2Loot = new();
    [Header("Stage 3 Loot")]
    public List<BossLootEntry> stage3Loot = new();

#if UNITY_EDITOR
    // === ������: 3�������� ��Ʈ�� �ڵ� ����(������ ID ���� ����) ===
    [ContextMenu("�ڵ� �ʱ�ȭ(3 ��������)")]
    private void Editor_AutoInit_AllStages()
    {
        stage1Loot = LootTableEditorUtility.GenerateStageEntries();
        stage2Loot = LootTableEditorUtility.GenerateStageEntries();
        stage3Loot = LootTableEditorUtility.GenerateStageEntries();

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[BossLootTable] 3 Stage �ڵ� �ʱ�ȭ �Ϸ�: {name}");
    }

    // === ������: ���� ������ �����ϸ鼭 ID�� īŻ�α� �������� �� ���� ===
    [ContextMenu("ID�� ���ΰ�ħ(īŻ�α�)")]
    private void Editor_RefreshIdsOnly_AllStages()
    {
        var catalog = ResourcesItemMetaCatalog.LoadDefault();
        if (!catalog)
        {
            Debug.LogWarning("[BossLootTable] ItemMetaCatalog�� �����ϴ�. Resources/Meta/ItemMetaCatalog ���� �ʿ�.");
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
        Debug.Log($"[BossLootTable] ID�� ���ΰ�ħ �Ϸ�: {name}");
    }

    // === ������: Relic ��� Ȯ�� �⺻�� ä���(���� ����) ===
    [ContextMenu("Relic ��� Ȯ�� �⺻�� ä���(1~3��������)")]
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

        // ���� �⺻��(���ϸ� ����)
        Ensure(1, 0.80f, 0.18f, 0.02f);
        Ensure(2, 0.60f, 0.30f, 0.10f);
        Ensure(3, 0.50f, 0.35f, 0.15f);

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[BossLootTable] Relic ��� Ȯ�� �⺻�� ���� �Ϸ�: {name}");
    }
#endif
}
