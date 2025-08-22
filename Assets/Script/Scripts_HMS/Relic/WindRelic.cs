using InterfaceRelic;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WindRelic : BaseRelic, IOnAcquireRelic, IOnRemoveRelic
{
    Modifier myMod;
    List<ScopedMod> applyingMods = new List<ScopedMod>();
    float atkSpeed => statData.GetStat(Stats.BasicRelicStat.AtkSpeed);
    public override void Init(SO_RelicMetaData metaData, Inventory inv)
    {
        base.Init(metaData, inv);
        myMod = new Modifier(ModifierType.PercentAdd, Stats.BaseWeaponStat.attackSpeed, -atkSpeed, this);
    }

    void IOnAcquireRelic.Invoke(Player player)
    {
        applyingMods.Add(inv.ApplyModifier(myMod, ModTargetKind.CurrentWeapon));
    }

    void IOnRemoveRelic.Invoke(Player player)
    {
        foreach (var mod in applyingMods) 
        {
            inv.RemoveModifier(mod);
        }    
        applyingMods.Clear();
    }
}
