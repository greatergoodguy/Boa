using System;
using System.Collections.Generic;

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
}
