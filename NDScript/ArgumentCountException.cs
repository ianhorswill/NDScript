using System;
using NDScript.Syntax;

namespace NDScript
{
    public class ArgumentCountException(string message, Function function, int expectedCount, object?[] actualArgs) : ArgumentException(message)
    {
        public Function Function = function;
        public readonly int ExpectedCount = expectedCount;
        public readonly object? ActualArguments = actualArgs;

        public static void Check(int expected, object?[] args, Function f, State s, AstNode? site=null, CallStack? stack = null)
        {
            if (args.Length == expected)
                return;
            var ex = new ArgumentCountException($"Wrong number of arguments to {f.Name}.  Expected {expected}, got {args.Length}.",
                f, expected, args);
            throw site != null? new ExecutionException(site, s, stack, ex): ex;
        }
    }
}