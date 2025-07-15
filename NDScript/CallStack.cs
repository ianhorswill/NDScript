using System;
using System.Diagnostics;
using System.Text;

namespace NDScript
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class CallStack
    {
        public readonly CallStack? Parent;
        public readonly Function Function;
        public readonly object?[] Arguments;
        public readonly int Depth;

        public static int MaxDepth = 100;

        public CallStack(Function function, object?[] arguments, CallStack? parent)
        {
            Function = function;
            Arguments = arguments;
            Parent = parent;
            Depth = parent?.Depth+1 ?? 1;
            if (Depth > MaxDepth)
                throw new Exception($"Maximum recursion depth exceeded.  Call stack:\n{this.ToString()}");
        }

        public sealed override string ToString()
        {
            var b = new StringBuilder();
            for (var frame = this; frame != null; frame = frame.Parent)
            {
                b.Append(frame.Function.Name);
                b.Append('(');
                var first = true;
                foreach (var a in frame.Arguments)
                {
                    if (first)
                        first = false;
                    else
                        b.Append(", ");
                    Printing.Format(a, b, true);
                }

                b.AppendLine(")");
            }

            return b.ToString();
        }

        internal string DebuggerDisplay => ToString();
    }
}
