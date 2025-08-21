using System;
using System.Linq;
using System.Reflection;

public static class FactoryTypeResolver
{
    public static bool TryResolveType(string typeName, Type mustImplement, out Type type)
    {
        type = null;
        if (string.IsNullOrEmpty(typeName)) return false;

        // 1) 직행
        type = Type.GetType(typeName, throwOnError: false);
        if (type == null)
        {
            // 2) 모든 로드된 어셈블리에서 검색 (유니티 환경에서 흔함)
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(typeName, throwOnError: false);
                if (type != null) break;
            }
        }

        if (type == null) return false;
        if (mustImplement != null && !mustImplement.IsAssignableFrom(type)) { type = null; return false; }
        if (type.GetConstructor(Type.EmptyTypes) == null) { type = null; return false; } // 매개변수 없는 생성자 필수
        return true;
    }
}