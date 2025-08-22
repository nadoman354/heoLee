using UnityEngine;

public static class CombatResolver
{
    public static DamageResult Resolve(in DamageRequest req, StatBlock attackerStats)
    {
        var result = new DamageResult();

        // ũ��
        float chance = Mathf.Clamp01(req.critChance);
        float mult = Mathf.Max(1f, req.critMultiplier);
        result.isCritical = req.canCrit && (Random.value < chance);
        float dmgMul = result.isCritical ? mult : 1f;

        // HP (���/���� ������ ���⼭ �߰�)
        result.finalHpDamage = Mathf.Max(0f, req.baseDamage * dmgMul);

        // �׷α�: ������(�÷��̾�) ���� ���� ( + ���� �� ��(�ջ���) )
        float plus = attackerStats.GetStat(Stats.GroggyKeys.GainAdd);
        float mulSum = Mathf.Max(0f, attackerStats.GetStat(Stats.GroggyKeys.GainMulSum));
        result.finalGroggyAdd = Mathf.Max(0f, (req.groggyPower + plus) * (1f + mulSum));

        return result;
    }

    // �׷α� ���ӽð� = (�⺻ + ��(+)) �� (1 + ��(��%))
    public static float ComputeGroggyDuration(StatBlock baseStats /*���� or ������*/)
    {
        float baseDur = baseStats.GetStat(Stats.BasicBossStat.GroggyBaseDuration);
        float plus = baseStats.GetStat(Stats.GroggyKeys.DurationAdd);
        float mulSum = Mathf.Max(0f, baseStats.GetStat(Stats.GroggyKeys.DurationMulSum));
        return Mathf.Max(0.05f, (baseDur + plus) * (1f + mulSum));
    }
}
