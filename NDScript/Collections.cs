using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using NDScript.Syntax;

namespace NDScript
{


    public static class Collections
    {
        public static bool IsSingleton(ICollection<object?> set) => set.Count == 1;
        public static bool IsEmpty(ICollection<object?> set) => set.Count == 0;

        public static IList ConvertToList(object? value, State s, string errorMessage, AstNode? site = null, CallStack? stack = null)
        {
            switch (value)
            {
                case StateElement:
                    return (ImmutableArray<object?>)s.Global[
                        ArgumentTypeException.CastObject<ArrayExpression>(value, typeof(IList), errorMessage, site,
                            stack)]!;
                case IList l:
                    return l;
                case IEnumerable<object?> ie:
                    return new List<object?>(ie);
                default:
                    var ex = new ArgumentTypeException(errorMessage, typeof(IList), value);
                    throw site != null? new ExecutionException(site, stack, ex): ex;
            }
        }

        private static GeneralPrimitive? chooseElement;

        internal static void MakePrimitives()
        {
            chooseElement = new GeneralPrimitive(
                "chooseElement", false,
                (args, site, state, stack, k) =>
                {
                    ArgumentCountException.Check(1, args, chooseElement!, site, stack);
                    var collection = ConvertToList(args[0], state, "Argument to chooseElement() is not a collection");
                    if (collection is not object?[] elements)
                    {
                        elements = new object?[collection.Count];
                        collection.CopyTo(elements, 0);
                    }

                    var offset = ChooseStatement.Random.Next(elements.Length);
                    for (var count = 0; count < elements.Length; count++)
                    {
                        var e = elements[(count + offset) % elements.Length];
                        if (k(e, state))
                            return true;
                    }

                    return false;
                });

            new DeterministicPrimitive<ICollection<object?>, bool>("isSingleton", IsSingleton);
            new DeterministicPrimitive<ICollection<object?>, bool>("isEmpty", IsEmpty);

            new DeterministicPrimitive<ICollection<object?>, int>("sizeOf",
                c =>
                    c.Count);
            new DeterministicPrimitive<object?, ICollection<object?>, bool>(
                "contains",
                (o, c) => c.Contains(o));
        }
    }
}