namespace NDScript.Syntax
{
    public class AssignmentExpression(SettableExpression lvalue, Expression value) : Expression([lvalue, value])
    {
        public readonly SettableExpression LValue = lvalue;
        public readonly Expression Value = value;

        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
            => Value.Execute(s, r, (v, ns) => LValue.Set(v, ns, r, k));
    }
}
