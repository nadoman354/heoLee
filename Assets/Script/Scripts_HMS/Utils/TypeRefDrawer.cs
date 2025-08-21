#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TypeRefAttribute))]
public sealed class TypeRefDrawer : PropertyDrawer
{
    // ĳ��: ���̽�Ÿ�� �� �ĺ� Ÿ�� ����Ʈ
    static readonly Dictionary<(Type baseType, bool reqCtor), List<Type>> _cache = new();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use string with [TypeRef]");
            return;
        }

        var attr = (TypeRefAttribute)attribute;
        var list = GetCandidates(attr.BaseType, attr.RequireDefaultCtor);

        // ǥ�ÿ� �̸� ���
        var display = new List<string> { "<None>" };
        display.AddRange(list.Select(PrettyName));

        // ���� ���� �ε���
        var curIndex = 0;
        var currentType = EditorTypeResolver.ResolveOrNull(property.stringValue);
        if (currentType != null)
        {
            var idx = list.IndexOf(currentType);
            if (idx >= 0) curIndex = idx + 1; // +1: <None> ����
        }

        EditorGUI.BeginProperty(position, label, property);
        var newIndex = EditorGUI.Popup(position, label.text, curIndex, display.ToArray());
        if (newIndex != curIndex)
        {
            if (newIndex == 0)
                property.stringValue = string.Empty; // None
            else
                property.stringValue = list[newIndex - 1].AssemblyQualifiedName;
        }
        EditorGUI.EndProperty();
    }

    static List<Type> GetCandidates(Type baseType, bool requireDefaultCtor)
    {
        var key = (baseType, requireDefaultCtor);
        if (_cache.TryGetValue(key, out var cached)) return cached;

        // UnityEditor.TypeCache: �����Ϳ��� �ſ� ���� Ÿ�� �˻�
        var types = UnityEditor.TypeCache.GetTypesDerivedFrom(baseType)
            .Where(t => !t.IsAbstract && !t.IsGenericType && t.IsClass)
            .Where(t => !requireDefaultCtor || t.GetConstructor(Type.EmptyTypes) != null)
            // ������ ����/���� Ŭ���� ����
            .Where(t => t.IsPublic || t.IsNestedPublic)
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name)
            .ToList();

        _cache[key] = types;
        return types;
    }

    static string PrettyName(Type t)
    {
        // ǥ�� �̸� ����ȭ: Namespace.TypeName ��
        if (string.IsNullOrEmpty(t.Namespace)) return t.Name;
        return $"{t.Namespace}.{t.Name}";
    }
}

/// <summary>�����Ϳ� Ÿ�� ������(��ȿ�� �˻�)</summary>
public static class EditorTypeResolver
{
    public static bool TryResolveType(string aqn, Type mustImplement, out Type type)
    {
        type = ResolveOrNull(aqn);
        if (type == null) return false;
        if (mustImplement != null && !mustImplement.IsAssignableFrom(type))
            return false;
        if (type.GetConstructor(Type.EmptyTypes) == null)
            return false;
        return true;
    }

    public static Type ResolveOrNull(string aqn)
    {
        if (string.IsNullOrEmpty(aqn)) return null;

        // 1) ����
        var t = Type.GetType(aqn, throwOnError: false);
        if (t != null) return t;

        // 2) ����� ��ü ��ĵ
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            t = asm.GetType(aqn, throwOnError: false);
            if (t != null) return t;
        }
        return null;
    }
}
#endif
