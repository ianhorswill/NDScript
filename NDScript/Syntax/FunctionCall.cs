using System;
using System.Linq;

namespace NDScript.Syntax
{
    public class FunctionCall(Expression func, Expression[] arguments) : Expression(arguments.Append(func).Cast<AstNode>().ToArray())
    {
        public readonly Expression Function = func;
        public readonly Expression[] Arguments = arguments;



        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
        {
            var argIndex = 0;
            var actualArguments = new object?[Arguments.Length];
            Function? function;

            bool ArgEvaluated(object? value, State newState)
            {
                actualArguments[argIndex++] = value;
                if (argIndex < Arguments.Length)
                    return Arguments[argIndex].Execute(newState, r, ArgEvaluated);
                return function.Call(actualArguments, newState,
                    (returnValue, finalState) =>
                        k(returnValue, s.ReplaceGlobal(finalState.Global)));
            }

            return Function.Execute(s, r, (fn, state) =>
            {
                function = fn as Function;
                if (function == null)
                    throw new Exception($"Attempt to call {fn}, which is not a function");
                if (actualArguments.Length == 0)
                    return function.Call(actualArguments, state,
                        (returnValue, finalState) => k(returnValue, s.ReplaceGlobal(finalState.Global)));
                return Arguments[0].Execute(state, r, ArgEvaluated);
            });
        }
    }
}