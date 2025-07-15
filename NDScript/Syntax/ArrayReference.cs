using System;
using System.Collections.Immutable;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class ArrayReference(Expression array, Expression index) : SettableExpression([array, index])
    {
        public readonly Expression Array = array;
        public readonly Expression Index = index;
        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k)
        {
            return Array.Execute(s, stack, r, (array, ns) => Index.Execute(ns, stack, r, (index, fs) =>
            {
                switch (array)
                {
                    case StateElement:
                        var si = ArgumentTypeException.CastObject<ArrayExpression>(array, typeof(Array),
                            "Argument to array reference was not an array");
                        var i = ArgumentTypeException.Cast<int>(index, "Index of array reference isn't an integer");
                        return k(((ImmutableArray<object?>)fs[si]!)[i], fs);

                    case Grid g:
                        var p = ArgumentTypeException.Cast<Position>(index, "Index of grid reference isn't a position");
                        return k(g.GetCell(p, fs), fs);

                    default:
                        throw new ArgumentException("Argument to array reference in not a collection");
                }
            }));
        }

        public override bool Set(object? value, State s, CallStack? stack, Continuation r, Continuation k)
        {
            return Array.Execute(s, stack, r, (array, ns) => Index.Execute(ns, stack, r, (index, fs) =>
            {
                switch (array)
                {
                    case StateElement:
                        var si = ArgumentTypeException.CastObject<ArrayExpression>(array, typeof(Array),
                            "Argument to array reference was not an array");
                        var i = ArgumentTypeException.Cast<int>(index, "Index of array reference isn't an integer");
                        return k(value, fs.Set(si, ((ImmutableArray<object?>)fs[si]!).SetItem(i, value)));

                    case Grid g:
                        var p = ArgumentTypeException.Cast<Position>(index, "Index of grid reference isn't a position");
                        return k(value, g.SetCell(p, fs, value));

                    default:
                        throw new ArgumentException("Argument to array assignment in not a collection");
                }
            }));
        }
    }
}
