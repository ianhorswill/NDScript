using System;

namespace NDScript.Syntax
{
    public class UnaryOperatorExpression(Func<object?, object?> operation, Expression argument) : Expression([argument])
    {
        public readonly Func<object?, object?> Operation = operation;
        public readonly Expression Argument = argument;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k) 
            => Argument.Execute(s, stack, r, (arg, newState) => k(Operation(arg), newState));

        public static Func<object?, object?> Wrap<T>(Func<T, object?> typed, string name)
            => arg => typed(ArgumentTypeException.Cast<T>(arg, name, "1"));

        public static object? Negate(object? arg)
        {
            if (arg is int n)
                return -n;
            return -ArgumentTypeException.CastSingle(arg, "-", "1");
        }

        public static object? Not(object? arg)
        {
            if (arg is bool b)
                return !b;
            throw new ArgumentTypeException("Arguments of ! must be Booleans", typeof(bool), arg);
        }

    }
}
