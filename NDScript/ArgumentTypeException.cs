using System;

namespace NDScript
{
    public class ArgumentTypeException(string message, Type expectedType, object? actualValue) : ArgumentException(message)
    {
        public readonly Type ExpectedType = expectedType;
        public readonly object? ActualValue = actualValue;

        public static T Cast<T>(object? value, string message)
        {
            if (value is T typed)
                return typed;
            throw new ArgumentTypeException(message, typeof(T), value);
        }

        public static T Cast<T>(object? value, string operation, string argumentName)
        {
            if (value is T typed)
                return typed;
            throw new ArgumentTypeException($"Argument {argumentName} to {operation} was expected to be a {typeof(T).Name}, but was instead the value {value}.", typeof(T), value);
        }

        public static float CastSingle(object? value, string operation, string argumentName)
        {
            return value switch
            {
                float f => f,
                int i => i,
                double d => (float)d,
                _ => throw new ArgumentTypeException(
                    $"Argument {argumentName} to {operation} was expected to be a number, but was instead the value {value}.",
                    typeof(float), value)
            };
        }

        public static StateElement CastObject<T>(object? value, Type fakeType, string message)
        {
            var si = value as StateElement;
            if (si == null || !(si.Description is T))
                throw new ArgumentTypeException($"{message}.  Expected {fakeType.Name}, got {value ?? "(null)"}",
                    fakeType, value);
            return si;
        }
    }
}
