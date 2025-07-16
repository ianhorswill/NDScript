using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class Block(int sourceLine, Statement[] statements) : Statement(sourceLine, statements)
    {
        public readonly Statement[] Statements = statements;

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k) =>
            ExecuteFrom(0, s, stack, r, k);

        private bool ExecuteFrom(int i, State s, CallStack? stack, Continuation r, Continuation k)
        {
            if (i == Statements.Length)
                // done
                return k(null, s);
            // Ignore result and run next statement
            return Statements[i].Execute(s, stack, r, (_, ns) => ExecuteFrom(i + 1, ns, stack, r, k));
        }
    }
}
