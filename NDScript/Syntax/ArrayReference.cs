using System;
using System.Collections.Immutable;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class ArrayReference(Expression array, Expression index) : SettableExpression([array, index])
    {
        public readonly Expression Array = array;
        public readonly Expression Index = index;
        public override bool Execute(State s, Continuation r, Continuation k)
        {
            return Array.Execute(s, r, (array, ns) => Index.Execute(ns, r, (index, fs) =>
            {
                var si = ArgumentTypeException.CastObject<ArrayExpression>(array, typeof(Array),
                    "Argument to array reference was not an array");
                var i = ArgumentTypeException.Cast<int>(index, "Index of array reference isn't an integer");
                return k(((ImmutableArray<object?>)fs[si]!)[i], fs);
            }));
        }

        public override bool Set(object? value, State s, Continuation r, Continuation k)
        {
            return Array.Execute(s, r, (array, ns) => Index.Execute(ns, r, (index, fs) =>
            {
                var si = ArgumentTypeException.CastObject<ArrayExpression>(array, typeof(Array),
                    "Argument to array reference was not an array");
                var i = ArgumentTypeException.Cast<int>(index, "Index of array reference isn't an integer");
                return k(value, fs.Set(si, ((ImmutableArray<object?>)fs[si]!).SetItem(i, value)));
            }));
        }
    }
}
