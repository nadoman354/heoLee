using enums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NormalLootEntry
{
    [Range(0f, 1f)] public float probability = 0f;  // ���� ����ġ(�� 1 ����)
    public ItemType itemType;                       // None = �̵��
    public List<string> possibleItemIds = new();    // None�̸� ����
}

[CreateAssetMenu(menuName = "Game/NormalLootTable")]
public class NormalLootTable : ScriptableObject
{
    [Header("Relic ����: ���������� ��� Ȯ��")]
    public List<StageRelicRarityChance> rarityChances = new();

    [Header("��� ���̺� ����(�����̳� ����)")]
    public List<NormalLootEntry> entries = new();

#if UNITY_EDITOR
    // === ������: �⺻ ������ �ڵ� ä��� ===
    [ContextMenu("�ڵ� �ʱ�ȭ(�⺻ ����)")]
    private void Editor_AutoFill_Default()
    {
        // �ʿ�� �⺻ ���� ���� ����
        LootTableEditorUtility.FillNormal(this, noneProb: 0.65f, weaponProb: 0.05f, relicProb: 0.05f, consumProb: 0.25f);
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[NormalLootTable] �⺻ ������ �ڵ� �ʱ�ȭ �Ϸ�: {name}");
    }

    // === ������: ���� entries �����ϸ鼭 ID�� īŻ�α׿��� ���� ��ܿ��� ===
    [ContextMenu("ID�� ���ΰ�ħ(īŻ�α�)")]
    private void Editor_RefreshIdsOnly()
    {
        var catalog = ResourcesItemMetaCatalog.LoadDefault();
        if (!catalog)
        {
            Debug.LogWarning("[NormalLootTable] ItemMetaCatalog�� �����ϴ�. Resources/Meta/ItemMetaCatalog ���� �ʿ�.");
            return;
        }

        // ���� ����, �� ������ Ÿ�Կ� �´� ID�� ����
        foreach (var e in entries)
        {
            if (e.itemType == ItemType.None) continue;
            e.possibleItemIds = catalog.GetIdsByType(e.itemType);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[NormalLootTable] ID�� ���ΰ�ħ �Ϸ�: {name}");
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
        Debug.Log($"[NormalLootTable] Relic ��� Ȯ�� �⺻�� ���� �Ϸ�: {name}");
    }
#endif
}
