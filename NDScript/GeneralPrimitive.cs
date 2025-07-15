using System;

namespace NDScript
{
    public class GeneralPrimitive(string name, Func<object?[], State, CallStack?, NDScript.Continuation, bool> implementation) : PrimitiveBase(name)
    {
        public readonly Func<object?[], State, CallStack?, NDScript.Continuation, bool> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, CallStack? stack, NDScript.Continuation k)
        {
            return Implementation(arguments, s, stack, k);
        }
    }
}