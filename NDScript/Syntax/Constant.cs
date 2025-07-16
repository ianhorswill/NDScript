using System;

namespace NDScript.Syntax
{
    public class Constant(int sourceLine, object? value) : Expression(sourceLine, [])
    {
        public readonly object? Value = value;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k) => k(Value, s);
    }
}
