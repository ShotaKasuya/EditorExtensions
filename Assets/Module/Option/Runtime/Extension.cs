namespace Module.Option.Runtime
{
    public static class Extension
    {
        public static Option<T> ToOption<T>(this T value)
        {
            return Option<T>.Some(value);
        }
    }
}