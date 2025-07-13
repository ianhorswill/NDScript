using System;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class VariableReference(string name) : SettableExpression([])
    {
        public readonly StateElement Name = (StateElement)name;

        public override bool Execute(State s, Continuation r, Continuation k) => k(s[Name], s);

        public override bool Set(object? value, State s, Continuation r, Continuation k) => k(value, s.Set(Name, value));
    }
}
