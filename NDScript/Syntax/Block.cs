using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class Block(Statement[] statements) : Statement(statements)
    {
        public readonly Statement[] Statements = statements;

        public override bool Execute(State s, Continuation r, Continuation k) =>
            ExecuteFrom(0, s, r, k);

        private bool ExecuteFrom(int i, State s, Continuation r, Continuation k)
        {
            if (i == Statements.Length)
                // done
                return k(null, s);
            // Ignore result and run next statement
            return Statements[i].Execute(s, r, (_, ns) => ExecuteFrom(i + 1, ns, r, k));
        }
    }
}
