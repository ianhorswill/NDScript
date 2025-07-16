using System;
using NDScript.Syntax;

namespace NDScript
{
    public class StatefulDeterministicPrimitive<TOut>(string name, Func<AstNode, CallStack, State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public StatefulDeterministicPrimitive(string name, Func<State, (TOut Result, State NewState)> implementation) :
            this(name, (site, stack, state) => implementation(state))
        { }

        public readonly Func<AstNode, CallStack, State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(0, arguments, this, callSite, stack);
            var result = Implementation(callSite, stack, s);
            return k(result.Result, result.NewState);
        }
    }

    public class StatefulDeterministicPrimitive<TIn1, TOut>(string name, Func<AstNode, CallStack, TIn1, State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public StatefulDeterministicPrimitive(string name, Func<TIn1, State, (TOut Result, State NewState)> implementation) :
            this(name, (site, stack, in1, state) => implementation(in1, state))
        { }

        public readonly Func<AstNode, CallStack, TIn1, State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(1, arguments, this);
            var result = Implementation(callSite, stack, 
                ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "arg 1", callSite, stack),
                s);
            return k(result.Result, result.NewState);
        }
    }

    public class StatefulDeterministicPrimitive<TIn1, TIn2, TOut>(string name, Func<AstNode, CallStack, TIn1, TIn2, State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public StatefulDeterministicPrimitive(string name, Func<TIn1, TIn2, State, (TOut Result, State NewState)> implementation) :
            this(name, (site, stack, in1, in2, state) => implementation(in1, in2, state))
        { }

        public readonly Func<AstNode, CallStack, TIn1, TIn2, State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(2, arguments, this, callSite, stack);
            var result = Implementation(callSite, stack, 
                ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "arg 1", callSite, stack),
                ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "arg 2", callSite, stack),
                s);
            return k(result.Result, result.NewState);
        }
    }

    public class StatefulDeterministicPrimitive<TIn1, TIn2, TIn3, TOut>(string name, Func<AstNode, CallStack, TIn1, TIn2, TIn3, State, (TOut Result, State NewState)> implementation) : PrimitiveBase(name, true)
    {
        public StatefulDeterministicPrimitive(string name, Func<TIn1, TIn2, TIn3, State, (TOut Result, State NewState)> implementation) :
            this(name, (site, stack, in1, in2, in3, state) => implementation(in1, in2, in3,state))
        { }

        public readonly Func<AstNode, CallStack, TIn1, TIn2, TIn3, State, (TOut Result, State NewState)> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            ArgumentCountException.Check(3, arguments, this, callSite, stack);
            var result = Implementation(callSite, stack, 
                ArgumentTypeException.Cast<TIn1>(arguments[0], Name, "arg 1", callSite, stack),
                ArgumentTypeException.Cast<TIn2>(arguments[1], Name, "arg 2", callSite, stack),
                ArgumentTypeException.Cast<TIn3>(arguments[2], Name, "arg 3", callSite, stack),
                s);
            return k(result.Result, result.NewState);
        }
    }
}