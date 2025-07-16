namespace NDScript.Syntax
{
    public class Fail(int sourceLine) : Expression(sourceLine, [])
    {
        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k) => false;
    }
}
