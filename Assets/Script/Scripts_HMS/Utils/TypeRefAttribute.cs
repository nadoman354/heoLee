using System;
using UnityEngine;

/// <summary>
/// ���ڿ� �ʵ�(AssemblyQualifiedName ����)�� �ٿ�,
/// Ư�� ���̽� Ÿ��/�������̽��� ������ Ŭ������ �� �� �ְ� ��.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class TypeRefAttribute : PropertyAttribute
{
    public readonly Type BaseType;
    public readonly bool RequireDefaultCtor;

    public TypeRefAttribute(Type baseType, bool requireDefaultCtor = true)
    {
        BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
        RequireDefaultCtor = requireDefaultCtor;
    }
}
