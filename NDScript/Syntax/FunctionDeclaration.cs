using System.Linq;

namespace NDScript.Syntax
{
    public class FunctionDeclaration(string name, string[] arguments, Block body) : Statement([body])
    {
        public readonly string Name = name;
        public readonly StateElement[] Arguments = arguments.Select(s => (StateElement)s).ToArray();
        public readonly Block Body = body;

        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
            => k(null, s.Create((StateElement)Name, new UserFunction(Name, Arguments, Body)));
    }
}