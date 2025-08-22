using UnityEngine;

public static class CombatResolver
{
    public static DamageResult Resolve(in DamageRequest req, StatBlock attackerStats)
    {
        var result = new DamageResult();

        // 크리
        float chance = Mathf.Clamp01(req.critChance);
        float mult = Mathf.Max(1f, req.critMultiplier);
        result.isCritical = req.canCrit && (Random.value < chance);
        float dmgMul = result.isCritical ? mult : 1f;

        // HP (방어/감쇠 있으면 여기서 추가)
        result.finalHpDamage = Mathf.Max(0f, req.baseDamage * dmgMul);

        // 그로기: 공격자(플레이어) 유물 보정 ( + 먼저 → ×(합산형) )
        float plus = attackerStats.GetStat(Stats.GroggyKeys.GainAdd);
        float mulSum = Mathf.Max(0f, attackerStats.GetStat(Stats.GroggyKeys.GainMulSum));
        result.finalGroggyAdd = Mathf.Max(0f, (req.groggyPower + plus) * (1f + mulSum));

        return result;
    }

    // 그로기 지속시간 = (기본 + Σ(+)) × (1 + Σ(×%))
    public static float ComputeGroggyDuration(StatBlock baseStats /*보스 or 공격자*/)
    {
        float baseDur = baseStats.GetStat(Stats.BasicBossStat.GroggyBaseDuration);
        float plus = baseStats.GetStat(Stats.GroggyKeys.DurationAdd);
        float mulSum = Mathf.Max(0f, baseStats.GetStat(Stats.GroggyKeys.DurationMulSum));
        return Mathf.Max(0.05f, (baseDur + plus) * (1f + mulSum));
    }
}
