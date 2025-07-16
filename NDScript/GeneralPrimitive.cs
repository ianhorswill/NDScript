using System;

namespace NDScript
{
    public class GeneralPrimitive(string name, bool isDeterministic, Func<object?[], State, CallStack?, NDScript.Continuation, bool> implementation) : PrimitiveBase(name, isDeterministic)
    {
        public readonly Func<object?[], State, CallStack?, NDScript.Continuation, bool> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, CallStack? stack, NDScript.Continuation k)
        {
            return Implementation(arguments, s, stack, k);
        }
    }
}