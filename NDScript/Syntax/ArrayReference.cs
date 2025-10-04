using System;
using System.Collections.Immutable;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class ArrayReference(int sourceLine, Expression array, Expression index) : SettableExpression(sourceLine, [array, index])
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
                            "Argument to array reference was not an array", fs);
                        var i = ArgumentTypeException.Cast<int>(index, "Index of array reference isn't an integer", fs);
                        return k(((ImmutableArray<object?>)fs[si]!)[i], fs);

                    case Grid g:
                        var p = ArgumentTypeException.Cast<Position>(index, "Index of grid reference isn't a position", fs);
                        return k(g.GetCell(p, fs), fs);

                    case CompoundObject o:
                        var member =
                            ArgumentTypeException.Cast<string>(index, "Member name must be a string", fs, this, stack);
                        var (memberValue, success) = o.GetMember(member, fs);
                        if (!success)
                            throw new ExecutionException(this, ns, stack,
                                new MissingMemberException($"Object has no field named {member}"));
                        return k(memberValue, fs);

                    default:
                        throw new ExecutionException(this, fs, stack, new ArgumentException($"Argument {Printing.Format(array, fs, true)} to array reference in not a collection"));
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
                            "Argument to array reference was not an array", fs);
                        var i = ArgumentTypeException.Cast<int>(index, "Index of array reference isn't an integer", fs);
                        return k(value, fs.Set(si, ((ImmutableArray<object?>)fs[si]!).SetItem(i, value)));

                    case Grid g:
                        var p = ArgumentTypeException.Cast<Position>(index, "Index of grid reference isn't a position", fs);
                        return k(value, g.SetCell(p, fs, value));

                    case CompoundObject o:
                        var member =
                            ArgumentTypeException.Cast<string>(index, "Member name must be a string", fs, this, stack);
                        return k(value, o.SetMember(member, value, fs));

                    default:
                        throw new ExecutionException(this, fs, stack, 
                            new ArgumentException(
                                $"Argument {Printing.Format(array, fs, true)} to array assignment in not a collection"));
                }
            }));
        }
    }
}
