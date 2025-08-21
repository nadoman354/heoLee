using System;
using UnityEngine;

public class BaseRelic
{
    protected SO_RelicMetaData metaData;
    protected SO_StatData statData;
    protected Inventory inv;
    public virtual void Init(SO_RelicMetaData metaData, Inventory inv)
    { 
        this.metaData = metaData;
        this.statData = metaData.stat;
    }

    internal SO_RelicMetaData GetMetaData()
    {
        return metaData;
    }
}
public abstract class BaseRelic<TMeta> : BaseRelic where TMeta : SO_RelicMetaData
{
    protected TMeta TypedMeta { get; private set; }

    public override void Init(SO_RelicMetaData meta, Inventory inv)
    {
        base.Init(meta, inv);
        TypedMeta = meta as TMeta
            ?? throw new System.ArgumentException($"[{GetType().Name}] {typeof(TMeta).Name} 메타가 필요합니다.");
    }
}