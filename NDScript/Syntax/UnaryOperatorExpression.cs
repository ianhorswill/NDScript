using System;

namespace NDScript.Syntax
{
    public class UnaryOperatorExpression(int sourceLine, Func<object?, State, object?> operation, Expression argument) : Expression(sourceLine, [argument])
    {
        public readonly Func<object?, State, object?> Operation = operation;
        public readonly Expression Argument = argument;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k) 
            => Argument.Execute(s, stack, r, (arg, newState) => k(Operation(arg, s), newState));

        public static Func<object?, State, object?> Wrap<T>(Func<T, object?> typed, string name)
            => (arg, s) => typed(ArgumentTypeException.Cast<T>(arg, name, "1", s));

        public static object? Negate(object? arg, State s)
        {
            if (arg is int n)
                return -n;
            return -ArgumentTypeException.CastSingle(arg, "-", "1", s);
        }

        public static object? Not(object? arg, State s)
        {
            if (arg is bool b)
                return !b;
            throw new ArgumentTypeException("Arguments of ! must be Booleans", typeof(bool), arg);
        }

    }
}
