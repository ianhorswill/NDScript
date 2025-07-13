using System.Collections.Immutable;
using System.Text;
using System.Web;

namespace NDScript
{
    public static class Printing
    {
        public static bool HtmlOutput = false;
        public static string Htmlify(string s) => HtmlOutput?HttpUtility.HtmlEncode(s).Replace("\n", "<br>"):s;
        public static readonly StateElement PrintState = new(typeof(Printing));

        private static ImmutableList<object> OutputList(State s) => (ImmutableList<object>)s[PrintState]!;

        public static string Output(State s)
        {
            var b = new StringBuilder();
            foreach (var fragment in OutputList(s))
                b.Append(fragment);
            return b.ToString();
        }

        public static State PrintRaw(string d, State s) => s.Set(PrintState, OutputList(s).Add(d));

        public static State Print(string d, State s) => PrintRaw(Htmlify(d), s);

        public static State Print(object? o, State s) => Print((o ?? "(null)").ToString(), s);

        public static State NewLine(State s) => Print("\n", s);
        public static State PrintImage(string url, State s) => s.Set(PrintState, OutputList(s).Add($"<img src=\"{url}\">"));

        internal static void MakePrimitives()
        {
            new GeneralPrimitive("print", (args, s, k) =>
            {
                var state = s;
                foreach (var item in args)
                    state = Printing.Print(item, state);
                return k(null, state);
            });

            new GeneralPrimitive("printLine", (args, s, k) =>
            {
                var state = s;
                foreach (var item in args)
                    state = Printing.Print(item, state);
                return k(null, Printing.NewLine(state));
            });

            new StatefulDeterministicPrimitive<string, object?>(
                "printImage",
                (url, state) => (null, Printing.PrintImage(url, state)));

            new DeterministicPrimitive<int, int, Position>("position", (x, y) => Position.At(x, y));
        }
    }
}
