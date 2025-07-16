using System.Linq;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class FunctionDeclaration(string name, bool isDeterministic, string[] arguments, Block body) : Statement([body])
    {
        public readonly string Name = name;
        public readonly bool IsDeterministic = isDeterministic;
        public readonly StateElement[] Arguments = arguments.Select(s => (StateElement)s).ToArray();
        public readonly Block Body = body;

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k)
            => k(null, s.Create((StateElement)Name, new UserFunction(Name, IsDeterministic, Arguments, Body)));
    }
}