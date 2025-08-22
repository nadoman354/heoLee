using UnityEngine;

[CreateAssetMenu(fileName = "SO_BossStatData", menuName = "Game/StatData/BossData")]
public class SO_BossStatData : SO_StatData
{
    protected override void Reset()
    {
        base.Reset();
        statList.Add(new Stat(Stats.BasicBossStat.MaxHp, 1200));
        statList.Add(new Stat(Stats.BasicBossStat.Damage, 25));
        statList.Add(new Stat(Stats.BasicBossStat.MoveSpeed, 3));
        statList.Add(new Stat(Stats.BasicBossStat.HealthRegenPerSec, 0));

        // �׷α� �⺻��
        statList.Add(new Stat(Stats.BasicBossStat.GroggyMax, 100));
        statList.Add(new Stat(Stats.BasicBossStat.GroggyBaseDuration, 3));

        // ����/���� �⺻(������ 0)
        statList.Add(new Stat(Stats.BasicBossStat.GroggyGainAdd, 0));
        statList.Add(new Stat(Stats.BasicBossStat.GroggyDurationAdd, 0));
        statList.Add(new Stat(Stats.BasicBossStat.GroggyGainMulSum, 0));   // %�� (��: 0.2 + 0.1)
        statList.Add(new Stat(Stats.BasicBossStat.GroggyDurationMulSum, 0));   // %��
    }
}
