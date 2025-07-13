using System.Collections.Immutable;
using System.Linq;

namespace NDScript
{
    public static class Sets
    {
        private static GeneralPrimitive? singletonValuePrimitive;

        internal static void MakePrimitives()
        {
            new GeneralPrimitive("setOf",
                (args, s, k) =>
                    k(args.ToImmutableHashSet(), s));

            new DeterministicPrimitive<ImmutableHashSet<object?>, ImmutableHashSet<object?>, ImmutableHashSet<object?>>(
                "union",
                (s1, s2) => s1.Union(s2));

            new DeterministicPrimitive<ImmutableHashSet<object?>, ImmutableHashSet<object?>, ImmutableHashSet<object?>>(
                "intersection",
                (s1, s2) => s1.Intersect(s2));

            singletonValuePrimitive = new GeneralPrimitive("singletonValue",
                (args, s, k) =>
                {
                    ArgumentCountException.Check(1, args, singletonValuePrimitive!);
                    var set = ArgumentTypeException.Cast<ImmutableHashSet<object?>>(args[0], "singletonValue", "set");
                    return Collections.IsSingleton(set) && k(set.First(), s);
                });
        }
    }
}
