using System;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class BinaryOperatorExpression(
        Func<object?, object?, object?> operation,
        Expression leftArgument,
        Expression rightArgument)
        : Expression([leftArgument, rightArgument])
    {
        public readonly Func<object?, object?, object?> Operation = operation;
        public readonly Expression LeftArgument = leftArgument;
        public readonly Expression RightArgument = rightArgument;

        public override bool Execute(State s, Continuation r, Continuation k) 
            => LeftArgument.Execute(s, r,
                (left, newState) =>
                    RightArgument.Execute(newState, r,
                        (right, finalState) => k(Operation(left, right), finalState)));

        public static object? Add(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                return nl + nr;
            return ArgumentTypeException.CastSingle(left, "+", "left")
                   + ArgumentTypeException.CastSingle(right, "+", "right");
        }

        public static object? Subtract(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                return nl - nr;
            return ArgumentTypeException.CastSingle(left, "-", "left")
                   - ArgumentTypeException.CastSingle(right, "-", "right");
        }

        public static object? Multiply(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                return nl * nr;
            return ArgumentTypeException.CastSingle(left, "*", "left")
                   * ArgumentTypeException.CastSingle(right, "*", "right");
        }

        public static object? Divide(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl%nr==0 ? (object)(nl / nr) : nl/(float)nr;
            return ArgumentTypeException.CastSingle(left, "/", "left")
                   / ArgumentTypeException.CastSingle(right, "/", "right");
        }

        public static object? Lt(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl < nr;
            return ArgumentTypeException.CastSingle(left, "/", "left")
                   < ArgumentTypeException.CastSingle(right, "/", "right");
        }

        public static object? Le(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl <= nr;
            return ArgumentTypeException.CastSingle(left, "/", "left")
                   <= ArgumentTypeException.CastSingle(right, "/", "right");
        }

        public static object? Gt(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl > nr;
            return ArgumentTypeException.CastSingle(left, "/", "left")
                   > ArgumentTypeException.CastSingle(right, "/", "right");
        }

        public static object? Ge(object? left, object? right)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl >= nr;
            return ArgumentTypeException.CastSingle(left, "/", "left")
                   >= ArgumentTypeException.CastSingle(right, "/", "right");
        }

        public static object? Eq(object? left, object? right) => Equals(left, right);
        public static object? Neq(object? left, object? right) => !Equals(left, right);
    }
}
