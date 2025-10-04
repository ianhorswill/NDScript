using System;
using NDScript.Syntax;

namespace NDScript
{
    public class DeterministicPrimitive<TOut>(string name, Func<AstNode, CallStack, State?, TOut> implementation) : PrimitiveBase(name, true)
    {
        public DeterministicPrimitive(string name, Func<TOut> implementation) : this(name, (site, stack, _) => implementation())
        { }

        public readonly Func<AstNode, CallStack, State, TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(0, arguments, this, s, callSite, stack);
            return k(Implementation(callSite, stack, s), s);
        }
    }

    public class DeterministicPrimitive<TIn1, TOut>(string name, Func<AstNode, CallStack, State, TIn1, TOut> implementation) : PrimitiveBase(name, true)
    {
        public DeterministicPrimitive(string name, Func<TIn1, TOut> implementation)
            : this(name, (site, stack, _, in1) => implementation(in1))
        { }
        
        public readonly Func<AstNode, CallStack, State, TIn1, TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(1, arguments, this, s, callSite, stack);
            var arg1 = ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "1", s, callSite, stack);
            return k(Implementation(callSite, stack, s, arg1), s);
        }
    }
    
    public class DeterministicPrimitive<TIn1, TIn2, TOut>(string name, Func<AstNode, CallStack, State, TIn1, TIn2, TOut> implementation) : PrimitiveBase(name, true)
    {
        public DeterministicPrimitive(string name, Func<TIn1, TIn2, TOut> implementation)
            : this(name, (site, stack, _, in1, in2) => implementation(in1, in2))
        { }
        
        public readonly Func<AstNode, CallStack, State, TIn1, TIn2, TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(2, arguments, this, s, callSite, stack);
            var arg1 = ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "1", s, callSite, stack);
            var arg2 = ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "2", s, callSite, stack);
            return k(Implementation(callSite, stack, s, arg1, arg2), s);
        }
    }
    
    public class DeterministicPrimitive<TIn1, TIn2, TIn3, TOut>(string name, Func<AstNode, CallStack, State, TIn1, TIn2, TIn3, TOut> implementation) : PrimitiveBase(name, true)
    {
        public DeterministicPrimitive(string name, Func<TIn1, TIn2, TIn3, TOut> implementation)
            : this(name, (site, stack, _, in1, in2, in3) => implementation(in1, in2, in3))
        { }
        
        public readonly Func<AstNode, CallStack, State, TIn1, TIn2, TIn3, TOut> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(2, arguments, this, s, callSite, stack);
            var arg1 = ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "1", s, callSite, stack);
            var arg2 = ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "2", s, callSite, stack);
            var arg3 = ArgumentTypeException.Cast<TIn3>(arguments[2], Name, "3", s, callSite, stack);
            return k(Implementation(callSite, stack, s,arg1, arg2, arg3), s);
        }
    }
}
