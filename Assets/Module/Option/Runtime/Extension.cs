namespace Module.Option.Runtime
{
    public static class Extension
    {
        public static Optional<T> ToOption<T>(this T value)
        {
            return Optional<T>.Some(value);
        }
    }
}