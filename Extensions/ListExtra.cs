using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;

namespace SuperstarsTestMod.Extensions;

public static class ListExtra
{
    private static void Resize<T>(this List<T> list, int sz, T c)
    {
        int cur = list.Count;
        if(sz < cur)
            list.RemoveRange(sz, cur - sz);
        else if(sz > cur)
        {
            if(sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                list.Capacity = sz;
            list.AddRange(Enumerable.Repeat(c, sz - cur));
        }
    }
    public static void Resize<T>(this List<T> list, int sz) where T : new()
    {
        Resize(list, sz, new T());
    }
    public static void Resize<T>(this List<string> list, int sz)
    {
        Resize(list, sz, "");
    }
}
