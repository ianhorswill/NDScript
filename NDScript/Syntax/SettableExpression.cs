using System;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public abstract class SettableExpression(Expression[] subexpressions) : Expression(subexpressions)
    {
        public abstract bool Set(object? value, State s, Continuation r, Continuation k);

        public static SettableExpression CheckSettable(Expression e)
        {
            if (e is SettableExpression s)
                return s;
            throw new Exception($"A {e.GetType().Name} cannot be used on the left hand side of an = expression");
        }
    }
}
