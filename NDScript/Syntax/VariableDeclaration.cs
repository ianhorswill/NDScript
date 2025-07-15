using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class VariableDeclaration(string name, Expression value) : Expression([value])
    {
        public readonly StateElement Name = (StateElement)name;
        public readonly Expression Value = value;

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k)
            => Value.Execute(s, stack, r, (v, ns) => k(v, ns.Create(Name, v)));
    }
}
