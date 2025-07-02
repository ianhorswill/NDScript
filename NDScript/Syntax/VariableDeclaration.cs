namespace NDScript.Syntax
{
    public class VariableDeclaration(string name, Expression value) : Expression([value])
    {
        public readonly StateElement Name = (StateElement)name;
        public readonly Expression Value = value;

        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
            => Value.Execute(s, r, (v, ns) => k(v, ns.Create(Name, v)));
    }
}
