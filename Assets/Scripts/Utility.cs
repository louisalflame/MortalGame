using Optional;
using Optional.Unsafe;
using UnityEngine;

public static class Utility
{
    public static bool TryGetValue<T>(this Option<T> opt, out T value)
    {
        value = opt.Match(
            v => v,
            () => default);
            
        return opt.HasValue;
    }
}

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}