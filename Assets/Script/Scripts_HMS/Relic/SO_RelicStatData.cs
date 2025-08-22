using UnityEngine;

namespace Stats
{
    public static class BasicRelicStat
    {
        public readonly static string HealthCondition = "HealthCondition";
        public readonly static string Damage = "Damage";
        public readonly static string MaxHp = "MaxHp";
        public readonly static string AtkSpeed = "AtkSpeed";
    }
}
[CreateAssetMenu(fileName = "SO_RelicStatData", menuName = "Game/StatData/RelicData/Base")]
public class SO_RelicStatData : SO_StatData
{
    protected override void Reset()
    {
        base.Reset();
        statList.Add(new Stat(Stats.BasicRelicStat.HealthCondition, 0));
        statList.Add(new Stat(Stats.BasicRelicStat.Damage, 0));
        statList.Add(new Stat(Stats.BasicRelicStat.MaxHp, 0));
        statList.Add(new Stat(Stats.BasicRelicStat.AtkSpeed, 0));
    }
}
