using System;
using NDScript.Syntax;

namespace NDScript
{
    public class GeneralPrimitive(string name, bool isDeterministic, Func<object?[], FunctionCall, State, CallStack?, NDScript.Continuation, bool> implementation) : PrimitiveBase(name, isDeterministic)
    {
        public readonly Func<object?[], FunctionCall, State, CallStack?, NDScript.Continuation, bool> Implementation = implementation;
        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack,
            NDScript.Continuation k)
        {
            return Implementation(arguments, callSite, s, stack, k);
        }
    }
}