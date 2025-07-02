using System.Collections.Immutable;
using System.Text;

namespace NDScript
{
    public static class Printing
    {
        public static readonly StateElement PrintState = new(typeof(Printing));

        private static ImmutableList<string> OutputList(State s) => (ImmutableList<string>)s[PrintState]!;

        public static string Output(State s)
        {
            var b = new StringBuilder();
            foreach (var fragment in OutputList(s))
                b.Append(fragment);
            return b.ToString();
        }

        public static State Print(string d, State s) => s.Set(PrintState, OutputList(s).Add(d));

        public static State Print(object? o, State s) => Print((o ?? "(null)").ToString(), s);

        public static State NewLine(State s) => Print("\n", s);
    }
}
