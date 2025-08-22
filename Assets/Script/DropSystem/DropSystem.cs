using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using enums; // ItemType, RarityType

public class DropSystem : MonoBehaviour
{
    public static DropSystem I { get; private set; }

    [Header("DIP 주입(비우면 싱글톤 기본값)")]
    [SerializeField] private MonoBehaviour rngProviderObj;     // IRng
    [SerializeField] private MonoBehaviour unlockProviderObj;  // IUnlockMask
    [SerializeField] private MonoBehaviour hubObj;             // IDroppedFactoryHub

    private IRng rng;
    private IUnlockMask unlock;
    private IDroppedFactoryHub hub;

    [Header("메타 카탈로그(비우면 기본 로드)")]
    [SerializeField] private ResourcesItemMetaCatalog metaCatalog;

    [Header("착지 제약(벽/함정 회피)")]
    [SerializeField] private LayerMask dropBlockMask;
    [SerializeField] private float landingClearance = 0.28f;
    [SerializeField] private float landingMinSep = 0.75f;

    [Header("착지 반경")]
    [SerializeField] private float normalRMin = 0.6f;
    [SerializeField] private float normalRMax = 1.6f;
    [SerializeField] private float bossRMin = 0.8f;
    [SerializeField] private float bossRMax = 2.2f;

    // RNG 도메인 문자열(충돌 방지)
    private const string DOMAIN_NORMAL = "drop_normal";
    private const string DOMAIN_BOSS = "drop_boss";

    // 폴백용 로컬 엔트로피(혹시 GameManager 없을 때)
    private int _localDropEntropy = 0;
    private int _localBossEntropy = 0;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);

        rng = (rngProviderObj as IRng) ?? RngService.I;
        unlock = (unlockProviderObj as IUnlockMask) ?? UnlockService.I;
        hub = (hubObj as IDroppedFactoryHub) ?? DroppedFactoryHub.I;

        if (!metaCatalog) metaCatalog = ResourcesItemMetaCatalog.LoadDefault();
    }

    // ========= 편의 오버로드(엔트로피 자동) =========
    public void DropNormal(Vector3 center, NormalLootTable table)
    {
        int stage = GameManager.I?.CurrentStage ?? 1;
        int entropy = GameManager.I?.NextDropEntropy() ?? (_localDropEntropy++);
        DropNormal(center, table, stage, entropy);
    }

    public void DropBoss(Vector3 center, BossLootTable table)
    {
        int stage = GameManager.I?.CurrentStage ?? 1;
        int entropy = GameManager.I?.NextDropEntropy() ?? (_localBossEntropy++);
        DropBoss(center, table, stage, entropy);
    }

    // ================= Normal =================
    /// <summary>노말: 라인 1개 가중치 추첨 → None이면 미드롭, 아니면 해당 라인의 풀에서 1개</summary>
    public void DropNormal(Vector3 center, NormalLootTable table, int stage, int entropy)
    {
        if (table == null || table.entries == null || table.entries.Count == 0) return;

        var r = rng.GetScoped(DOMAIN_NORMAL, stage, entropy);

        var line = RollNormalLine(table.entries, r);
        if (line == null || line.itemType == ItemType.None) return;
        if (line.possibleItemIds == null || line.possibleItemIds.Count == 0) return;

        List<string> pool;
        if (line.itemType == ItemType.Relic)
        {
            var rarity = RollRelicRarity(table.rarityChances, stage, r);
            var byRarity = FilterRelicByRarity(line.possibleItemIds, rarity);
            pool = unlock.FilterEligible(ItemType.Relic, byRarity);
        }
        else
        {
            pool = unlock.FilterEligible(line.itemType, line.possibleItemIds);
        }
        if (pool.Count == 0) return;

        string id = pool[r.Next(pool.Count)];

        var landing = ScatterPlanner2D.GenerateValid(
            1, center, r, normalRMin, normalRMax, landingMinSep, dropBlockMask, landingClearance
        )[0];

        var item = hub.SpawnFromId(line.itemType, id, center) as DroppedItem;
        item?.DropTo(landing, r);
    }

    public void DropNormal(Vector3 center, NormalLootTable table, int entropy)
        => DropNormal(center, table, GameManager.I?.CurrentStage ?? 1, entropy);

    // ================= Boss =================
    /// <summary>보스: 타입별 count 롤 → 전체 착지점 생성 → 각 타입 풀에서 뽑아 배치</summary>
    public void DropBoss(Vector3 center, BossLootTable table, int stage, int entropy)
    {
        if (table == null) return;

        var r = rng.GetScoped(DOMAIN_BOSS, stage, entropy);

        var entries = GetStageEntries(table, stage);
        if (entries == null || entries.Count == 0) return;

        var planned = new List<(BossLootEntry e, int count)>();
        int total = 0;
        foreach (var e in entries)
        {
            if (e.possibleItemIds == null || e.possibleItemIds.Count == 0) continue;
            int c = RollCount(e.dropCountChances, r);
            if (c > 0) { planned.Add((e, c)); total += c; }
        }
        if (total <= 0) return;

        var landings = ScatterPlanner2D.GenerateValid(
            total, center, r, bossRMin, bossRMax, landingMinSep, dropBlockMask, landingClearance
        );

        int k = 0;
        foreach (var p in planned)
        {
            for (int i = 0; i < p.count; i++)
            {
                List<string> pool;
                if (p.e.itemType == ItemType.Relic)
                {
                    var rarity = RollRelicRarity(table.rarityChances, stage, r);  // 픽마다 등급 롤
                    var byRarity = FilterRelicByRarity(p.e.possibleItemIds, rarity);
                    pool = unlock.FilterEligible(ItemType.Relic, byRarity);
                }
                else
                {
                    pool = unlock.FilterEligible(p.e.itemType, p.e.possibleItemIds);
                }
                if (pool.Count == 0) continue;

                string id = pool[r.Next(pool.Count)];
                var item = hub.SpawnFromId(p.e.itemType, id, center) as DroppedItem;
                item?.DropTo(landings[Mathf.Min(k, landings.Count - 1)], r);
                k++;
            }
        }
    }

    public void DropBoss(Vector3 center, BossLootTable table, int entropy)
        => DropBoss(center, table, GameManager.I?.CurrentStage ?? 1, entropy);

    // ================= Helpers =================
    private static NormalLootEntry RollNormalLine(List<NormalLootEntry> lines, System.Random rng)
    {
        float sum = 0f; foreach (var l in lines) sum += Mathf.Max(0f, l.probability);
        if (sum <= 0f) return null;

        double x = rng.NextDouble() * sum; float acc = 0f;
        foreach (var l in lines) { acc += Mathf.Max(0f, l.probability); if (x <= acc) return l; }
        return lines[^1];
    }

    private static List<BossLootEntry> GetStageEntries(BossLootTable t, int stage) => stage switch
    {
        1 => t.stage1Loot,
        2 => t.stage2Loot,
        3 => t.stage3Loot,
        _ => t.stage1Loot
    };

    private static int RollCount(List<DropCountChance> cs, System.Random r)
    {
        if (cs == null || cs.Count == 0) return 0;
        float sum = cs.Sum(x => Mathf.Max(0f, x.probability));
        if (sum <= 0f) return 0;
        double x = r.NextDouble() * sum; float acc = 0f;
        foreach (var c in cs) { acc += Mathf.Max(0f, c.probability); if (x <= acc) return c.count; }
        return cs[^1].count;
    }

    private static RarityType RollRelicRarity(List<StageRelicRarityChance> byStage, int stage, System.Random r)
    {
        if (byStage == null || byStage.Count == 0) return RarityType.Normal;
        var row = byStage.Find(x => x.stage == stage) ?? byStage[0];

        float sum = 0f;
        foreach (var rc in row.rarityChances) sum += Mathf.Max(0f, rc.probability);
        if (sum <= 0f) return RarityType.Normal;

        double x = r.NextDouble() * sum; float acc = 0f;
        foreach (var rc in row.rarityChances)
        {
            acc += Mathf.Max(0f, rc.probability);
            if (x <= acc) return rc.rarity;
        }
        return row.rarityChances[^1].rarity;
    }

    private List<string> FilterRelicByRarity(List<string> ids, RarityType rarity)
    {
        var res = new List<string>();
        if (!metaCatalog) return res;

        foreach (var id in ids)
            if (metaCatalog.TryGetRelic(id, out var so) && so.rarity == rarity)
                res.Add(id);

        return res;
    }
}
