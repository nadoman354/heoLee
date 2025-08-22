using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyStatData", menuName = "Game/StatData/EnemyData")]
public class SO_EnemyStatData : SO_StatData
{
    protected override void Reset()
    {
        base.Reset();
        statList.Add(new Stat(Stats.BasicEnemyStat.MaxHp, 50));
        statList.Add(new Stat(Stats.BasicEnemyStat.Damage, 8));
        statList.Add(new Stat(Stats.BasicEnemyStat.MoveSpeed, 2));
        statList.Add(new Stat(Stats.BasicEnemyStat.HealthRegenPerSec, 0));
    }
}