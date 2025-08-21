using InterfaceRelic;
using System.Collections.Generic;
using UnityEngine;

public class HealthRelic : BaseRelic, IOnAcquireRelic, IOnRemoveRelic
{
    List<Modifier> myAppliedMod = new List<Modifier>();
    void IOnAcquireRelic.Invoke(Player player)
    {
        Modifier mod = new Modifier(ModifierType.Flat, Stats.BasicPlayerStat.MaxHp, statData.GetStat(Stats.BasicRelicStat.MaxHp), this);
        player.AddModifier(mod);
    }

    void IOnRemoveRelic.Invoke(Player player)
    {
        foreach (Modifier mod in myAppliedMod)
        {
            player.RemoveModifier(mod);
        }
    }
}
