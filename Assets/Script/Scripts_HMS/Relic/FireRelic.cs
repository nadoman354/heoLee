using InterfaceRelic;
using UnityEngine;

public sealed class FireRelic : BaseRelic, IOnAcquireRelic, IOnRemoveRelic, IOnHpChanged
{
    ScopedMod _handle; // 적용한 모디 토큰(추적/해제 용)
    Modifier _mod;
    float _threshold;
    bool _applied;

    // * 스탯 참조 숏컷 *
    string targetStat => Stats.BasicPlayerStat.Damage; // 무기 공격력 <- 지금은 스탯이 Player에게 있지만, 곧 옮겨질 예정!
    float addedDamage => statData.GetStat(Stats.BasicRelicStat.Damage); // 무기 공격력 증가량

    public override void Init(SO_RelicMetaData meta, Inventory inv)
    {
        base.Init(meta, inv);
        _mod = new Modifier(ModifierType.Flat, targetStat, addedDamage, this);
        _threshold = statData.GetStat(Stats.BasicRelicStat.HealthCondition);
    }

    void IOnAcquireRelic.Invoke(Player p) => Evaluate(p);
    public void OnHpChanged(Player p, in HealthChangeContext ctx) => Evaluate(p);

    void Evaluate(Player p)
    {
        bool should = p.health.Current < _threshold;

        if (should && !_applied)
        {
            // “현재 장착 무기” 스코프로 한 번만 신청하면, 교체 시 자동으로 움직임
            _handle = inv.ApplyModifier(_mod, ModTargetKind.CurrentWeapon);
            _applied = true;
        }
        else if (!should && _applied)
        {
            inv.RemoveModifier(_handle);
            _applied = false;
        }
    }

    void IOnRemoveRelic.Invoke(Player p)
    {
        if (_applied) { inv.RemoveModifier(_handle); _applied = false; }
    }

}
