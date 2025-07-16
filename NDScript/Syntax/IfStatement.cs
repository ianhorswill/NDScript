using System;

namespace NDScript.Syntax
{
    public class IfStatement(int sourceLine, Expression condition, Statement consequent, Statement? alternative) 
        : Statement(sourceLine, alternative == null?[condition, consequent]:[condition, consequent, alternative])
    {
        public readonly Expression Condition = condition;
        public readonly Statement Consequent = consequent;
        public readonly Statement? Alternative = alternative;
        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k) =>
            Condition.Execute(s, stack, r, (c, ns) =>
            {
                if (!(c is bool b))
                    throw new ArgumentException($"Condition in if statement is not a Boolean");
                if (b)
                    return Consequent.Execute(ns, stack, r, k);
                if (Alternative != null)
                    return Alternative.Execute(ns, stack, r, k);
                return k(null, ns);
            });
    }
}
