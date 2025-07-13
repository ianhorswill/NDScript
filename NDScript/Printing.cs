using System.Collections;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Web;

namespace NDScript
{
    public static class Printing
    {
        public static bool HtmlOutput = false;
        public static string Htmlify(string s) => HtmlOutput?HttpUtility.HtmlEncode(s).Replace("\n", "<br>"):s;

        public const string DefaultImageExtension = ".png";

        public static string AddImageExtensionIfNecessary(string path) =>
            Path.HasExtension(path) ? path : Path.ChangeExtension(path, DefaultImageExtension);

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

        public static string Format(object? o)
        {
            var b = new StringBuilder();
            Format(o, b);
            return b.ToString();
        }

        private static void Format(object? o, StringBuilder b, bool escapeStrings = false)
        {
            switch (o)
            {
                case null:
                    b.Append("(null)");
                    break;

                case true:
                    b.Append("true");
                    break;

                case false:
                    b.Append("false");
                    break;

                case string s when escapeStrings:
                {
                    b.Append('"');
                    foreach (var c in s)
                    {
                        if (c == '\\' || c == '"')
                            b.Append('\\');
                        b.Append(c);
                    }
                    b.Append('"');
                    break;
                }

                case ImmutableHashSet<object?> s:
                {
                    b.Append("{");
                    var first = true;
                    foreach (var e in s)
                    {
                        if (first)
                            first = false;
                        else
                            b.Append(", ");
                        Format(e, b, true);
                    }

                    b.Append("}");
                    break;
                }

                case IList l:
                {
                    b.Append("[");
                    var first = true;
                    foreach (var e in l)
                    {
                        if (first)
                            first = false;
                        else
                            b.Append(", ");
                        Format(e, b, true);
                    }

                    b.Append("]");
                    break;
                }

                default:
                    b.Append(o);
                    break;
            }
        }

        public static State Print(object? o, State s) => Print(Format(o), s);

        public static State NewLine(State s) => Print("\n", s);
        public static State PrintImage(string url, State s) =>
            s.Set(PrintState, OutputList(s).Add($"<img src=\"{AddImageExtensionIfNecessary(url)}\">"));

        internal static void MakePrimitives()
        {
            new GeneralPrimitive("print", (args, s, k) =>
            {
                var state = s;
                foreach (var item in args)
                    state = Print(item, state);
                return k(null, state);
            });

            new GeneralPrimitive("printLine", (args, s, k) =>
            {
                var state = s;
                foreach (var item in args)
                    state = Print(item, state);
                return k(null, NewLine(state));
            });

            new StatefulDeterministicPrimitive<string, object?>(
                "printImage",
                (url, state) => (null, PrintImage(url, state)));

            new DeterministicPrimitive<int, int, Position>("position", (x, y) => Position.At(x, y));
        }
    }
}
