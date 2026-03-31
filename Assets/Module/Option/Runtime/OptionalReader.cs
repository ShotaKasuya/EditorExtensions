#nullable enable
using System.Runtime.CompilerServices;

namespace Module.Option.Runtime
{
    public readonly struct OptionalReader<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OptionalReader<T> Some(T value)
        {
            return new OptionalReader<T>(true, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OptionalReader<T> None()
        {
            return new OptionalReader<T>(false, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(out T? outValue)
        {
            outValue = _isSome ? _value : default;
            return _isSome;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Unwrap()
        {
            return _value!;
        }

        public bool IsSome => _isSome;
        public bool IsNone => !_isSome;

        private readonly bool _isSome;
        private readonly T? _value;

        private OptionalReader(bool isSome, T? value)
        {
            _isSome = isSome;
            _value = value;
        }

        public override string ToString()
        {
            if (IsSome)
            {
                return $"Some({_value!.ToString()})";
            }

            return "None";
        }
    }
}