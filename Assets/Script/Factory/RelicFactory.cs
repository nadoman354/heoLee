using System;

public class RelicFactory
{
    private static RelicFactory instance = new RelicFactory();
    public static RelicFactory Instance => instance;
    public BaseRelic CreateRelic(SO_RelicMetaData meta)
    {
        Type relicType = Type.GetType(meta.className); // "MyNamespace.FireRelic"
        if (relicType == null)
            throw new Exception($"Relic type not found: {meta.className}");
        BaseRelic relic = (BaseRelic)Activator.CreateInstance(relicType);
        relic.Init(meta);
        return relic;
    }

    internal Relic Create(SO_RelicMetaData meta)
    {
        throw new NotImplementedException();
    }
}

public class BaseRelic
{
    public void Init(SO_RelicMetaData meta)
    {

    }
}