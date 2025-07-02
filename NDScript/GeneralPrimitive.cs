using System;

namespace NDScript
{
    public class GeneralPrimitive(string name, Func<object?[], State, NDScript.Continuation, bool> implementation) : PrimitiveBase(name)
    {
        public readonly Func<object?[], State, NDScript.Continuation, bool> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, NDScript.Continuation k)
        {
            return Implementation(arguments, s, k);
        }
    }
}