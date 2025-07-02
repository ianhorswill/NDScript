using System;

namespace NDScript.Syntax
{
    public class WhileStatement(Expression condition, Statement body) 
        : Statement([condition, body])
    {
        public readonly Expression Condition = condition;
        public readonly Statement Body = body;

        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k) =>
            Condition.Execute(s, r, (c, ns) =>
            {
                if (!(c is bool b))
                    throw new ArgumentException($"Condition in if statement is not a Boolean");
                if (b)
                    return Body.Execute(ns, r, (_, nns) => Execute(nns, r, k));
                return k(null, ns);
            });
    }
}