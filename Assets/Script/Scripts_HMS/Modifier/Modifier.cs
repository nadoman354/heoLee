using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType { Flat, PercentAdd, Multifly}
public class Modifier
{
    public ModifierType type;
    public string targetStat;
    public float value;
    public object source;
    public Modifier(ModifierType type, string targetStat, float value, object source)
    {
        this.type = type;
        this.targetStat = targetStat;
        this.value = value;
        this.source = source;
    }
}