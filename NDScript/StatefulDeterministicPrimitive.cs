using System;

namespace NDScript
{
    public class StatefulDeterministicPrimitive<TOut>(string name, Func<State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public readonly Func<State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, CallStack? stack, NDScript.Continuation k)
        {
            ArgumentCountException.Check(0, arguments, this);
            var result = Implementation(s);
            return k(result.Result, result.NewState);
        }
    }

    public class StatefulDeterministicPrimitive<TIn1, TOut>(string name, Func<TIn1, State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public readonly Func<TIn1, State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, CallStack? stack, NDScript.Continuation k)
        {
            ArgumentCountException.Check(1, arguments, this);
            var result = Implementation(
                ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "arg 1"),
                s);
            return k(result.Result, result.NewState);
        }
    }

    public class StatefulDeterministicPrimitive<TIn1, TIn2, TOut>(string name, Func<TIn1, TIn2, State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public readonly Func<TIn1, TIn2, State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, CallStack? stack, NDScript.Continuation k)
        {
            ArgumentCountException.Check(2, arguments, this);
            var result = Implementation(
                ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "arg 1"),
                ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "arg 2"),
                s);
            return k(result.Result, result.NewState);
        }
    }

    public class StatefulDeterministicPrimitive<TIn1, TIn2, TIn3, TOut>(string name, Func<TIn1, TIn2, TIn3, State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public readonly Func<TIn1, TIn2, TIn3, State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, CallStack? stack, NDScript.Continuation k)
        {
            ArgumentCountException.Check(3, arguments, this);
            var result = Implementation(
                ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "arg 1"),
                ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "arg 2"),
                ArgumentTypeException.Cast<TIn3>(arguments[2], Name, "arg 3"),
                s);
            return k(result.Result, result.NewState);
        }
    }
}