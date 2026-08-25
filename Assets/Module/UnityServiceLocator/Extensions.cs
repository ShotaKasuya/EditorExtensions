using System.Runtime.CompilerServices;
using UnityEngine;

namespace Module.UnityServiceLocator
{
    internal static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T? OrNull<T>(this T obj) where T : Object?
            => obj ? obj : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T AssertNull<T>(this T? reference) where T : class
        {
            Debug.Assert(reference != null);
            return reference!;
        }
    }
}