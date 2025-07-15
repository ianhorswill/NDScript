using System;
using System.Collections.Generic;
using System.Linq;
using NDScript.Syntax;
using Sprache;

namespace NDScript
{
    // ReSharper disable once InconsistentNaming
    public static class NDScript
    {
        public delegate bool Continuation(object? value, State s);

        public static object? RunExpression(string code , params KeyValuePair<string, object?>[] bindings)
        {
            var state = new State(
                bindings.Select(p => new KeyValuePair<StateElement, object?>((StateElement)p.Key, p.Value)),
                null);
            var exp = Parser.Expression.End().Parse(code);
            object? result = null!;
            if (exp.Execute(state, null, NoReturnContinuation, (value, s) =>
                {
                    result = value;
                    if (result is StateElement si)
                        result = s[si];
                    return true;
                }))
                return result;
            throw new Exception("Expression failed");
        }

        public static object? RunProgram(string code)
        {
            var statements = Parser.StatementSequence.Parse(code);

            object? result = null;

            bool Done(object? v, State _)
            {
                result = v;
                return true;
            }

            if (!new Block(statements).Execute(State.Default, null, Done, Done))
                throw new Exception("Program failed");
            return result;
        }

        public static State StateAfterProgram(string code)
        {
            var statements = Parser.StatementSequence.End().Parse(StripComments(code));

            State result = State.Default;

            bool Done(object? _, State s)
            {
                result = s;
                return true;
            }

            Minimization.RemoveBudgetConstraints();

            if (!new Block(statements).Execute(State.Default, null, Done, Done))
                throw new Exception("Program failed");
            return result;
        }

        public static string ProgramOutput(string code) => Printing.Output(StateAfterProgram(code));

        private static bool NoReturnContinuation(object? value, State s)
        {
            throw new InvalidOperationException("Attempt to execute a return statement at top level");
        }

        private static string StripLineComment(string line)
        {
            var comment = line.IndexOf("//", StringComparison.Ordinal);
            return comment < 0 ? line : line.Substring(0, comment);
        }

        private static string StripComments(string code) => string.Join('\n', code.Split('\n').Select(StripLineComment));
    }
}
