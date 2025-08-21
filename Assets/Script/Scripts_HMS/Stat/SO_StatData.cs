using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SO_StatData : ScriptableObject
{
    public List<Stat> statList;
    protected virtual void Reset()
    {
        statList = new List<Stat>();
    }
    public float GetStat(string name)
    {
        foreach (Stat stat in statList)
        {
            if(stat.name == name)
            {
                return stat.value;
            }
        }
        return 0;
    }
}