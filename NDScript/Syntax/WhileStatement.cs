using System;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class WhileStatement(int sourceLine, Expression condition, Statement body) 
        : Statement(sourceLine, [condition, body])
    {
        public readonly Expression Condition = condition;
        public readonly Statement Body = body;

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k) =>
            Condition.Execute(s, stack, r, (c, ns) =>
            {
                if (!(c is bool b))
                    throw new ExecutionException(this, stack, new ArgumentException($"Condition in if statement is not a Boolean"));
                return b ? Body.Execute(ns, stack, r, (_, nns) => Execute(nns, stack, r, k)) : k(null, ns);
            });
    }
}