using System.Runtime.CompilerServices;
using UnityEngine;

namespace Module.UnityServiceLocator
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? OrNull<T>(this T obj) where T : Object?
            => obj ? obj : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertNull<T>(this T? reference) where T : class
        {
            Debug.Assert(reference != null);
            return reference!;
        }
    }
}