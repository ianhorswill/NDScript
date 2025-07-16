namespace NDScript.Syntax
{
    // ReSharper disable once CoVariantArrayConversion
    public class ChooseExpression(int sourceLine, Expression[] alternatives) : Expression(sourceLine, alternatives)
    {
        public readonly Expression[] Alternatives = alternatives;
        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k)
        {
            foreach (var a in Alternatives)
                if (a.Execute(s, stack, r, k))
                    return true;
            return false;
        }
    }
}
