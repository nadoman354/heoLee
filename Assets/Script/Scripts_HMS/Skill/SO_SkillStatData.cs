using UnityEngine;
namespace Stats
{
    public static class BaseSkillStat
    {
        public readonly static string coolTime = "coolTime";
        public readonly static string damage = "damage";
        public readonly static string maxStack = "maxStack";// ��ų ��Ÿ���� ���� ����� �� ����ϴ� ����
    }
}

[CreateAssetMenu(fileName = "SO_SkillStatData", menuName = "Game/StatData/SkillData/Base")]
public class SO_SkillStatData : SO_StatData
{
    protected override void Reset()
    {
        base.Reset();
        statList.Add(new Stat(Stats.BaseSkillStat.coolTime, 0));
        statList.Add(new Stat(Stats.BaseSkillStat.maxStack, 0));
        statList.Add(new Stat(Stats.BaseSkillStat.damage, 0));
    }
}
