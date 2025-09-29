using System;
using System.Linq;

namespace NDScript.Syntax
{
    public class FunctionCall(int sourceLine, Expression func, Expression[] arguments) : Expression(sourceLine, arguments.Append(func).Cast<AstNode>().ToArray())
    {
        public readonly Expression Function = func;
        public readonly Expression[] Arguments = arguments;



        public override bool Execute(State s, CallStack? caller, NDScript.Continuation r, NDScript.Continuation k)
        {
            var actualArguments = new object?[Arguments.Length];
            Function? function;

            bool DoCall(State state)
            {
                if (function.IsDeterministic)
                {
                    object? result = null;
                    State finalState = s;
                    if (function.Call(actualArguments, this, state, 
                            new CallStack(function, actualArguments, caller, this),
                            (returnValue, fs) =>
                            {
                                result = returnValue;
                                finalState = fs;
                                return true;
                            }))
                        return k(result, finalState);
                    return false;
                }
                return function.Call(actualArguments, this, state, 
                    new CallStack(function, actualArguments, caller, this),
                    (returnValue, finalState) =>
                        k(returnValue, s.ReplaceGlobal(finalState.Global)));
            }

            bool ArgEvaluated(int argIndex, object? value, State newState)
            {
                actualArguments[argIndex] = value;
                var next = argIndex + 1;
                if (next < actualArguments.Length)
                    return Arguments[next].Execute(newState, caller, r, 
                        (v,s) => ArgEvaluated(next, v, s));

                // Actually call it
                return DoCall(newState);
            }

            return Function.Execute(s, caller, r, (fn, state) =>
            {
                function = fn as Function;
                if (function == null)
                    throw new ExecutionException(this, caller, new Exception($"Attempt to call {Printing.Format(fn)}, which is not a function"));
                if (actualArguments.Length == 0)
                    return DoCall(state);

                return Arguments[0].Execute(state, caller, r, (v,s) => ArgEvaluated(0, v, s));
            });
        }
    }
}