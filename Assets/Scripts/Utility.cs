using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;
using UnityEngine;

public static class Utility
{
    public class Dictionary<T, V> : System.Collections.Generic.Dictionary<T, V>
    {
        public static readonly Dictionary<T, V> EMPTY = new ();
    }

    public static bool TryGetValue<T>(this Option<T> opt, out T value)
    {
        value = opt.Match(
            v => v,
            () => default);

        return opt.HasValue;
    }

    public static Option<(T1, T2)> Combine<T1, T2>(this Option<T1> first, Option<T2> second)
    {
        return first.Match(
            v1 => second.Match(
                v2 => Option.Some((v1, v2)),
                () => Option.None<(T1, T2)>()),
            () => Option.None<(T1, T2)>());
    }


    public static bool Eval<T>(this SetConditionType conditionType, IEnumerable<T> items, System.Func<T, bool> predicate)
    {
        return conditionType switch
        {
            SetConditionType.AnyInside => items.Any(predicate),
            SetConditionType.AllInside => items.All(predicate),
            SetConditionType.AnyOutside => items.Any(item => !predicate(item)),
            SetConditionType.AllOutside => items.All(item => !predicate(item)),
            _ => false
        };
    }
    
    public static IEnumerable<T> WrapAsEnumerable<T>(this T item)
    {
        yield return item;
    }
}

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}