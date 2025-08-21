using System.Collections.Generic;
using System;
using UnityEngine;

public interface ILogicSkillFactory
{
    ISkillLogic Create(SkillContext ctx);
}
public class LogicSkillFactory : ILogicSkillFactory
{
    // ���ڿ� �� Type ĳ��
    private static readonly Dictionary<string, Type> _typeCache = new();

    public ISkillLogic Create(SkillContext ctx)
    {
        SO_SkillMetaData meta = ctx.metadata;
        ICapabilities cap = ctx.caps;
        if (meta == null) throw new ArgumentNullException(nameof(meta));


        var key = meta.className;
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException($"{meta.name}: LogicTypeName is empty.");

        if (!_typeCache.TryGetValue(key, out var t))
        {
            if (!FactoryTypeResolver.TryResolveType(key, typeof(ISkillLogic), out t))
                throw new InvalidOperationException($"IWeaponLogic logic type '{key}' not found or invalid.");
            _typeCache[key] = t;
        }

        var logic = (ISkillLogic)Activator.CreateInstance(t); // �Ķ���� ���� ������ �ʼ�
        logic.Initialize(new SkillContext(meta, cap));
        return logic;
    }
}