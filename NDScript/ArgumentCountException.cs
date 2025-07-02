using System;

namespace NDScript
{
    public class ArgumentCountException(string message, Function function, int expectedCount, object?[] actualArgs) : ArgumentException(message)
    {
        public Function Function = function;
        public readonly int ExpectedCount = expectedCount;
        public readonly object? ActualArguments = actualArgs;

        public static void Check(int expected, object?[] args, Function f)
        {
            if (args.Length == expected)
                return;
            throw new ArgumentCountException($"Wrong number of arguments to {f.Name}.  Expected {expected}, got {args.Length}.",
                f, expected, args);
        }
    }
}