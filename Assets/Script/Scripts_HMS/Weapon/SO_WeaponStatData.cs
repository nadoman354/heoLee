using UnityEngine;

namespace Stats
{
    public static class BaseWeaponStat
    {
        public readonly static string attackPower = "attackPower";
        public readonly static string staggerPower = "staggerPower";
        public readonly static string attackSpeed = "attackSpeed";
    }
}

[CreateAssetMenu(fileName = "SO_WeaponStatData", menuName = "Game/StatData/WeaponData/Base")]
public class SO_WeaponStatData : SO_StatData
{
    protected override void Reset()
    {
        base.Reset();
        statList.Add(new Stat(Stats.BaseWeaponStat.attackPower, 0));
        statList.Add(new Stat(Stats.BaseWeaponStat.staggerPower, 0));
        statList.Add(new Stat(Stats.BaseWeaponStat.attackSpeed, 0));
    }
}
