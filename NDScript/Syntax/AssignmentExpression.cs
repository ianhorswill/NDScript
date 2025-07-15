namespace NDScript.Syntax
{
    public class AssignmentExpression(SettableExpression lvalue, Expression value) : Expression([lvalue, value])
    {
        public readonly SettableExpression LValue = lvalue;
        public readonly Expression Value = value;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k)
            => Value.Execute(s, stack, r, (v, ns) => LValue.Set(v,  ns, stack, r, k));
    }
}
