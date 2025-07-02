using System;

namespace NDScript
{
    public class StatefulDeterministicPrimitive<TOut>(string name, Func<State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name)
    {
        public readonly Func<State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, NDScript.Continuation k)
        {
            ArgumentCountException.Check(0, arguments, this);
            var result = Implementation(s);
            return k(result.Result, result.NewState);
        }
    }
}