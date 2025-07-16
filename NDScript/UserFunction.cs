using System;
using System.Collections.Generic;
using System.Linq;
using NDScript.Syntax;
using static NDScript.NDScript;

namespace NDScript
{
    public class UserFunction(string name, bool isDeterministic, StateElement[] arguments, Statement body) : Function(name, isDeterministic)
    {
        public readonly StateElement[] Arguments = arguments;
        public readonly Statement Body = body;

        public override bool Call(object?[] arguments, FunctionCall callSite, State s, CallStack stack, Continuation k)
        {
            if (Arguments.Length != arguments.Length)
                throw new Exception(
                    $"Wrong number of arguments to {Name}: expected {Arguments.Length}, got {arguments.Length}");
            var frame = new State(
                Arguments.Zip(arguments, (A, a) => new KeyValuePair<StateElement, object?>(A, a)),
                s.Global);
            var done = (Continuation)((v, ns) => k(v, s.ReplaceGlobal(ns.Global)));
            return Body.Execute(frame, stack, done, done);
        }
    }
}
