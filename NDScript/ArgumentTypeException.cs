using NDScript.Syntax;
using System;
using static NDScript.Printing;

namespace NDScript
{
    public class ArgumentTypeException(string message, Type expectedType, object? actualValue) : ArgumentException(message)
    {
        public readonly Type ExpectedType = expectedType;
        public readonly object? ActualValue = actualValue;

        public static T Cast<T>(object? value, string message, AstNode? site=null, CallStack? stack = null)
        {
            if (value is T typed)
                return typed;
            var ex = new ArgumentTypeException($"{message}; expected {Format(value, true)} to be a {typeof(T)}.", typeof(T), value);
            throw site != null? new ExecutionException(site, stack, ex): ex;
        }

        public static T Cast<T>(object? value, string operation, string argumentName, AstNode? site=null, CallStack? stack = null)
        {
            if (value is T typed)
                return typed;
            var ex = new ArgumentTypeException($"Argument {argumentName} to {operation} was expected to be a {typeof(T).Name}, but was instead the value {Printing.Format(value, true)}.", typeof(T), value);
            throw site != null? new ExecutionException(site, stack, ex): ex;
        }

        public static float CastSingle(object? value, string operation, string argumentName, AstNode? site=null, CallStack? stack = null)
        {
            switch (value)
            {
                case float f:
                    return f;
                case int i:
                    return i;
                case double d:
                    return (float)d;
                default:
                    var ex = new ArgumentTypeException(
                        $"Argument {argumentName} to {operation} was expected to be a number, but was instead the value {Format(value, true)}.",
                        typeof(float), value);
                    throw site != null? new ExecutionException(site, stack, ex): ex;
            }
        }

        public static StateElement CastObject<T>(object? value, Type fakeType, string message, AstNode? site=null, CallStack? stack = null)
        {
            var si = value as StateElement;
            if (si == null || !(si.Description is T))
            {
                var ex = new ArgumentTypeException($"{message}.  Expected {fakeType.Name}, got {Format(value, true)}",
                    fakeType, value);
                throw site != null? new ExecutionException(site, stack, ex): ex;
            }

            return si;
        }
    }
}
