using System;

namespace NDScript
{
    public class DeterministicPrimitive<TOut>(string name, Func<TOut> implementation) : PrimitiveBase(name)
    {
        public readonly Func<TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, NDScript.Continuation k)
        {
            ArgumentCountException.Check(0, arguments, this);
            return k(Implementation(), s);
        }
    }

    public class DeterministicPrimitive<TIn1, TOut>(string name, Func<TIn1, TOut> implementation) : PrimitiveBase(name)
    {
        public readonly Func<TIn1, TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, NDScript.Continuation k)
        {
            ArgumentCountException.Check(1, arguments, this);
            var arg1 = ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "1");
            return k(Implementation(arg1), s);
        }
    }
    
    public class DeterministicPrimitive<TIn1, TIn2, TOut>(string name, Func<TIn1, TIn2, TOut> implementation) : PrimitiveBase(name)
    {
        public readonly Func<TIn1, TIn2, TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, NDScript.Continuation k)
        {
            ArgumentCountException.Check(2, arguments, this);
            var arg1 = ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "1");
            var arg2 = ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "2");
            return k(Implementation(arg1, arg2), s);
        }
    }
    
    public class DeterministicPrimitive<TIn1, TIn2, TIn3, TOut>(string name, Func<TIn1, TIn2, TIn3, TOut> implementation) : PrimitiveBase(name)
    {
        public readonly Func<TIn1, TIn2, TIn3, TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, State s, NDScript.Continuation k)
        {
            ArgumentCountException.Check(2, arguments, this);
            var arg1 = ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "1");
            var arg2 = ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "2");
            var arg3 = ArgumentTypeException.Cast<TIn3>(arguments[2], Name, "3");
            return k(Implementation(arg1, arg2, arg3), s);
        }
    }
}
