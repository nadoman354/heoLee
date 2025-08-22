using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class StatBlock 
{
    Dictionary<string, float> baseStats;

    List<Modifier> flatModifiers;    // +-
    List<Modifier> percentModifiers; // +%
    List<Modifier> multiflyModifiers;// *%
    Dictionary<string, float> finalStats;
    HashSet<string> dirtyStats;
    internal Action<string> OnStatChanged;

    public void Init(SO_StatData data)
    {
        baseStats = new Dictionary<string, float>();
        flatModifiers = new List<Modifier>();
        percentModifiers = new List<Modifier>();
        multiflyModifiers = new List<Modifier>();
        finalStats = new Dictionary<string, float>();
        dirtyStats = new HashSet<string> ();

        foreach(var stat in data.statList)
        {
            baseStats.Add(stat.name, stat.value);
            finalStats.Add(stat.name, stat.value);
        }
    }
    public void SetStat(string name, float value)
    {
        if(baseStats.ContainsKey(name))
            baseStats[name] = value;
        else
            baseStats.Add(name, value);
        SetDirty(name);
        if (OnStatChanged != null) OnStatChanged(name);
    }
    public float GetStat(string name)
    {
        if (finalStats.ContainsKey(name) == false) return -999;

        ReCalcStat(name);
        return finalStats[name];   
    }
    public void AddModifier(Modifier mod)
    {
        switch (mod.type)
        {
            case ModifierType.Flat:
                flatModifiers.Add(mod);
                break;
            case ModifierType.PercentAdd:
                percentModifiers.Add(mod);
                break;
            case ModifierType.Multifly:
                multiflyModifiers.Add(mod);
                break;
        }

        SetDirty(mod.targetStat);
        if(OnStatChanged != null) OnStatChanged(mod.targetStat);
    }
    public void RemoveModifier(Modifier mod)
    {
        switch (mod.type)
        {
            case ModifierType.Flat:
                flatModifiers.Remove(mod);
                break;
            case ModifierType.PercentAdd:
                percentModifiers.Remove(mod);
                break;
            case ModifierType.Multifly:
                multiflyModifiers.Remove(mod);
                break;
        }
        SetDirty(mod.targetStat);
        if (OnStatChanged != null) OnStatChanged(mod.targetStat);
    }
    public void ReCalcStat(string target)
    {
        if(dirtyStats.Contains(target) == false) return;
        if (baseStats.ContainsKey(target) == false) return;

        float tempStat = baseStats[target];
        foreach (Modifier flat in flatModifiers)
            if (flat.targetStat == target)
                tempStat += flat.value;
        float totalPercent = 0;
        foreach (Modifier percent in percentModifiers)
            if (percent.targetStat == target)
                totalPercent += percent.value;
        tempStat *= (1 + totalPercent);
        foreach (Modifier mult in multiflyModifiers)
            if (mult.targetStat == target)
                tempStat *= mult.value;
        finalStats[target] = tempStat;
        dirtyStats.Remove(target);
    }
    public void ReCalcAllStat()
    {
        foreach(string dirty in dirtyStats)
        {
            if (baseStats.ContainsKey(dirty) == false) continue;

            float tempStat = baseStats[dirty];
            foreach (Modifier flat in flatModifiers)
                if (flat.targetStat == dirty)
                    tempStat += flat.value;
            float totalPercent = 0;
            foreach (Modifier percent in percentModifiers)
                if (percent.targetStat == dirty)
                    totalPercent += percent.value;
            tempStat *= (1 + totalPercent);
            foreach (Modifier mult in multiflyModifiers)
                if (mult.targetStat == dirty)
                    tempStat *= mult.value;
            finalStats[dirty] = tempStat;
        }
        dirtyStats.Clear();
    }
    private void SetDirty(string dirty)
    {
        dirtyStats.Add(dirty);
    }
}
