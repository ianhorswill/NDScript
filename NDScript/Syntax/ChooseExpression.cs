namespace NDScript.Syntax
{
    public class ChooseExpression(Expression[] alternatives) : Expression(alternatives)
    {
        public readonly Expression[] Alternatives = alternatives;
        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
        {
            foreach (var a in Alternatives)
                if (a.Execute(s, r, k))
                    return true;
            return false;
        }
    }
}
