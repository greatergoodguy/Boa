using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    public static void ZipDo<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Action<T1, T2> action) {
        using(var e1 = first.GetEnumerator())
        using(var e2 = second.GetEnumerator()) {
            while (e1.MoveNext() && e2.MoveNext()) {
                action(e1.Current, e2.Current);
            }
        }
    }

    // Allows using .ForEach() on arrays and IEnumerables without having to call .ToList() first
    public static int ForEach<T>(this IEnumerable<T> list, Action<int, T> action) {
        if (action == null) throw new ArgumentNullException("action");

        var index = 0;

        foreach (var elem in list)
            action(index++, elem);

        return index;
    }

    public static bool HasComponent<T>(this GameObject obj) {
        return obj.GetComponent(typeof(T)) != null;
    }
    
    public static string Truncate(this string value, int maxLength) {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
}
