using System;

namespace NDScript.Syntax
{
    public class Constant(object? value) : Expression([])
    {
        public readonly object? Value = value;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k) => k(Value, s);
    }
}
