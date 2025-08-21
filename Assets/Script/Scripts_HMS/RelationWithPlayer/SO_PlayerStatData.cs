using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    public static class BasicPlayerStat
    {
        public readonly static string MaxHp = "MaxHp";
        public readonly static string Damage = "Damage";
    }
}

[Serializable]
public struct Stat
{
    public string name;
    public float value;

    public Stat(string name, int value)
    {
        this.name = name;
        this.value = value;
    }
}
[CreateAssetMenu(fileName = "SO_PlayerStatData", menuName = "Game/StatData/PlayerData")]
public class SO_PlayerStatData : SO_StatData
{
    protected override void Reset()
    {
        base.Reset();
        statList.Add(new Stat(Stats.BasicPlayerStat.MaxHp, 100));
    }
}

