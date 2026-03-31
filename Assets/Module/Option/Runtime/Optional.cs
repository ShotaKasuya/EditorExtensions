#nullable enable

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Module.Option.Runtime
{
    [Serializable]
    public struct Optional<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Some(T value)
        {
            return new Optional<T>(true, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> None()
        {
            return new Optional<T>(false, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(out T? outValue)
        {
            outValue = isSome ? value : default;
            return isSome;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Unwrap()
        {
            return value!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<TMap> Map<TMap>(Func<T, TMap> convert)
        {
            return Optional<TMap>.Some(convert(Unwrap()));
        }

        public bool IsSome => isSome;
        public bool IsNone => !isSome;

        [SerializeField] private bool isSome;
        [SerializeField] private T? value;

        private Optional(bool isSome, T? value)
        {
            this.isSome = isSome;
            this.value = value;
        }

        public override string ToString()
        {
            if (IsSome)
            {
                return $"Some({value!.ToString()})";
            }

            return "None";
        }

        public static implicit operator OptionalReader<T>(Optional<T> value)
        {
            return value.IsSome
                ? OptionalReader<T>.Some(value.Unwrap())
                : OptionalReader<T>.None();
        }
    }
}