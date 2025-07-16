namespace NDScript.Syntax
{
    public class AssignmentExpression(int sourceLine, SettableExpression lvalue, Expression value) : Expression(sourceLine, [lvalue, value])
    {
        public readonly SettableExpression LValue = lvalue;
        public readonly Expression Value = value;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k)
            => Value.Execute(s, stack, r, (v, ns) => LValue.Set(v,  ns, stack, r, k));
    }
}
