using System;
using System.Linq;
using System.Reflection;

public static class FactoryTypeResolver
{
    public static bool TryResolveType(string typeName, Type mustImplement, out Type type)
    {
        type = null;
        if (string.IsNullOrEmpty(typeName)) return false;

        // 1) ����
        type = Type.GetType(typeName, throwOnError: false);
        if (type == null)
        {
            // 2) ��� �ε�� ��������� �˻� (����Ƽ ȯ�濡�� ����)
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(typeName, throwOnError: false);
                if (type != null) break;
            }
        }

        if (type == null) return false;
        if (mustImplement != null && !mustImplement.IsAssignableFrom(type)) { type = null; return false; }
        if (type.GetConstructor(Type.EmptyTypes) == null) { type = null; return false; } // �Ű����� ���� ������ �ʼ�
        return true;
    }
}