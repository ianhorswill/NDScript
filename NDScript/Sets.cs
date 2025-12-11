using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace NDScript
{
    public static class Sets
    {
        private static GeneralPrimitive? _singletonValuePrimitive;

        internal static void MakePrimitives()
        {
            // ReSharper disable ObjectCreationAsStatement
            new GeneralPrimitive("setOf", true,
                (args, site, s, _, k) =>
                    k(args.ToImmutableHashSet(), s));

            new DeterministicPrimitive<ImmutableHashSet<object?>, ImmutableHashSet<object?>, ImmutableHashSet<object?>>(
                "union",
                (s1, s2) => s1.Union(s2));

            new DeterministicPrimitive<ImmutableHashSet<object?>, ImmutableHashSet<object?>, ImmutableHashSet<object?>>(
                "intersection",
                (s1, s2) => s1.Intersect(s2));

            new DeterministicPrimitive<ImmutableHashSet<object?>, ImmutableHashSet<object?>, ImmutableHashSet<object?>>(
                "except",
                (s1, s2) => s1.Except(s2));

            // ReSharper restore ObjectCreationAsStatement

            _singletonValuePrimitive = new GeneralPrimitive("singletonValue", true,
                (args, site, s, stack, k) =>
                {
                    ArgumentCountException.Check(1, args, _singletonValuePrimitive!, s, site, stack);
                    var set = ArgumentTypeException.Cast<ICollection<object?>>(args[0], "singletonValue", "set", s, site, stack);
                    return Collections.IsSingleton(set) && k(set.First(), s);
                });
        }
    }
}
