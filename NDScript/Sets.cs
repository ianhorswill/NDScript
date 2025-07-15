using System.Collections.Immutable;
using System.Linq;

namespace NDScript
{
    public static class Sets
    {
        private static GeneralPrimitive? _singletonValuePrimitive;

        internal static void MakePrimitives()
        {
            // ReSharper disable ObjectCreationAsStatement
            new GeneralPrimitive("setOf",
                (args, s, _, k) =>
                    k(args.ToImmutableHashSet(), s));

            new DeterministicPrimitive<ImmutableHashSet<object?>, ImmutableHashSet<object?>, ImmutableHashSet<object?>>(
                "union",
                (s1, s2) => s1.Union(s2));

            new DeterministicPrimitive<ImmutableHashSet<object?>, ImmutableHashSet<object?>, ImmutableHashSet<object?>>(
                "intersection",
                (s1, s2) => s1.Intersect(s2));
            // ReSharper restore ObjectCreationAsStatement

            _singletonValuePrimitive = new GeneralPrimitive("singletonValue",
                (args, s, _, k) =>
                {
                    ArgumentCountException.Check(1, args, _singletonValuePrimitive!);
                    var set = ArgumentTypeException.Cast<ImmutableHashSet<object?>>(args[0], "singletonValue", "set");
                    return Collections.IsSingleton(set) && k(set.First(), s);
                });
        }
    }
}
