using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class Return(Expression value) : Expression([value])
    {
        public readonly Expression Value = value;

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k)
            => Value.Execute(s, stack, r, r);
    }
}
