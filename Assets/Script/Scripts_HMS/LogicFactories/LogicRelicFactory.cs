using System.Collections.Generic;
using System;
using UnityEngine;
using InterfaceRelic;

public interface ILogicRelicFactory
{
    BaseRelic Create(SO_RelicMetaData meta, Inventory inventory);
}

public class LogicRelicFactory : ILogicRelicFactory
{
    // 문자열 → Type 캐시
    private static readonly Dictionary<string, Type> _typeCache = new();

    public BaseRelic Create(SO_RelicMetaData meta, Inventory inventory)
    {
        if (meta == null) throw new ArgumentNullException(nameof(meta));

        var key = meta.className;
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException($"{meta.name}: LogicTypeName is empty.");

        if (!_typeCache.TryGetValue(key, out var t))
        {
            if (!FactoryTypeResolver.TryResolveType(key, typeof(BaseRelic), out t))
                throw new InvalidOperationException($"BaseRelic logic type '{key}' not found or invalid.");
            _typeCache[key] = t;
        }

        var logic = (BaseRelic)Activator.CreateInstance(t); // 파라미터 없는 생성자 필수
        logic.Init(meta, inventory);

        return logic;
    }
}
