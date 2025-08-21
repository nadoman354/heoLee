using System;
using System.Collections.Generic;

public interface ILogicWeaponFactory
{
    IWeaponLogic Create(SO_WeaponMetaData meta, WeaponView view, ICapabilities cap);
}
public sealed class LogicWeaponFactory : ILogicWeaponFactory
{
    // 문자열 → Type 캐시
    private static readonly Dictionary<string, Type> _typeCache = new();

    public IWeaponLogic Create(SO_WeaponMetaData meta, WeaponView view, ICapabilities cap)
    {
        if (meta == null) throw new ArgumentNullException(nameof(meta));


        var key = meta.className;
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException($"{meta.name}: LogicTypeName is empty.");

        if (!_typeCache.TryGetValue(key, out var t))
        {
            if (!FactoryTypeResolver.TryResolveType(key, typeof(IWeaponLogic), out t))
                throw new InvalidOperationException($"IWeaponLogic logic type '{key}' not found or invalid.");
            _typeCache[key] = t;
        }

        var logic = (IWeaponLogic)Activator.CreateInstance(t); // 파라미터 없는 생성자 필수
        logic.Initialize(new WeaponContext(meta, view, cap));
        return logic;
    }
}