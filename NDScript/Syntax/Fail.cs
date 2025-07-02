namespace NDScript.Syntax
{
    public class Fail() : Expression([])
    {
        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k) => false;

        public static readonly Fail Singleton = new();
    }
}
