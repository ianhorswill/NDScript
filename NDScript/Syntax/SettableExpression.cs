using System;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public abstract class SettableExpression(int sourceLine, Expression[] subexpressions) : Expression(sourceLine, subexpressions)
    {
        public abstract bool Set(object? value, State s, CallStack? stack, Continuation r, Continuation k);

        public static SettableExpression CheckSettable(Expression e)
        {
            if (e is SettableExpression s)
                return s;
            throw new Exception($"A {e.GetType().Name} cannot be used on the left hand side of an = expression");
        }
    }
}
