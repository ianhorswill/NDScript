using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class BinaryOperatorExpression(
        int sourceLine, 
        Func<object?, object?, State, object?> operation,
        Expression leftArgument,
        Expression rightArgument)
        : Expression(sourceLine, [leftArgument, rightArgument])
    {
        public readonly Func<object?, object?, State, object?> Operation = operation;
        public readonly Expression LeftArgument = leftArgument;
        public readonly Expression RightArgument = rightArgument;

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k) 
            => LeftArgument.Execute(s, stack, r,
                (left, newState) =>
                    RightArgument.Execute(newState, stack, r,
                        (right, finalState) => k(Operation(left, right, s), finalState)));

        public static object? Add(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                return nl + nr;
            return ArgumentTypeException.CastSingle(left, "+", "left", s)
                   + ArgumentTypeException.CastSingle(right, "+", "right", s);
        }

        public static object? Subtract(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                return nl - nr;
            return ArgumentTypeException.CastSingle(left, "-", "left", s)
                   - ArgumentTypeException.CastSingle(right, "-", "right", s);
        }

        public static object? Multiply(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                return nl * nr;
            return ArgumentTypeException.CastSingle(left, "*", "left", s)
                   * ArgumentTypeException.CastSingle(right, "*", "right", s);
        }

        public static object? Divide(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl%nr==0 ? (object)(nl / nr) : nl/(float)nr;
            return ArgumentTypeException.CastSingle(left, "/", "left", s)
                   / ArgumentTypeException.CastSingle(right, "/", "right", s);
        }

        public static object? Lt(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl < nr;
            return ArgumentTypeException.CastSingle(left, "/", "left", s)
                   < ArgumentTypeException.CastSingle(right, "/", "right", s);
        }

        public static object? Le(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl <= nr;
            return ArgumentTypeException.CastSingle(left, "/", "left", s)
                   <= ArgumentTypeException.CastSingle(right, "/", "right", s);
        }

        public static object? Gt(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl > nr;
            return ArgumentTypeException.CastSingle(left, "/", "left", s)
                   > ArgumentTypeException.CastSingle(right, "/", "right", s);
        }

        public static object? Ge(object? left, object? right, State s)
        {
            if (left is int nl && right is int nr)
                // Promote to floating-point division if there's a remainder
                return nl >= nr;
            return ArgumentTypeException.CastSingle(left, "/", "left", s)
                   >= ArgumentTypeException.CastSingle(right, "/", "right", s);
        }

        public static object Eq(object? left, object? right, State _) => CheckEquality(left, right);

        public static bool CheckEquality(object? left, object? right)
        {
            if (left is ImmutableHashSet<object?> sl && right is ImmutableHashSet<object?> sr)
                return sl.SetEquals(sr);
            if (left is IList<object?> al && right is IList<object?> ar)
            {
                if (al.Count != ar.Count) return false;
                for (var i = 0; i < al.Count; i++)
                    if (!CheckEquality(al[i], ar[i]))
                        return false;
                return true;
            }
            return Equals(left, right);
        }

        public static object? Neq(object? left, object? right, State _) => !CheckEquality(left, right);
    }
}
