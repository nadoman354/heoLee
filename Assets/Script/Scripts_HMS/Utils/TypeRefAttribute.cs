using System;
using UnityEngine;

/// <summary>
/// 문자열 필드(AssemblyQualifiedName 저장)에 붙여,
/// 특정 베이스 타입/인터페이스를 구현한 클래스만 고를 수 있게 함.
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
