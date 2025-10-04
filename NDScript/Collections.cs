using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NDScript.Syntax;

namespace NDScript
{


    public static class Collections
    {
        public static bool IsSingleton(ICollection<object?> set) => set.Count == 1;
        public static bool IsEmpty(ICollection<object?> set) => set.Count == 0;

        public static IList<object?> ConvertToList(object? value, State s, string errorMessage, AstNode? site = null, CallStack? stack = null)
        {
            switch (value)
            {
                case StateElement:
                    return (ImmutableArray<object?>)s.Global[
                        ArgumentTypeException.CastObject<ArrayExpression>(value, typeof(IList), errorMessage, s, site,
                            stack)]!;
                case IList<object?> l:
                    return l;
                case IEnumerable<object?> ie:
                    return new List<object?>(ie);
                default:
                    var ex = new ArgumentTypeException(errorMessage, typeof(IList), value);
                    throw site != null? new ExecutionException(site, s, stack, ex): ex;
            }
        }

        private static GeneralPrimitive? _chooseElement;

        internal static void MakePrimitives()
        {
            _chooseElement = new GeneralPrimitive(
                "chooseElement", false,
                (args, site, state, stack, k) =>
                {
                    var argCount = args.Length;
                    if (argCount == 0 || argCount > 2)
                        ArgumentCountException.Check(2, args, _chooseElement!, state, site, stack);
                    var collection = ConvertToList(args[0], state, "Argument to chooseElement() is not a collection");
                    var weights = argCount == 2
                        ? ArgumentTypeException.Cast<CompoundObject>(args[1], "chooseElement", "weights", state,
                            site, stack).CurrentDictionary(state):null;

                    var elements = collection!.Select(e => new KeyValuePair<float,object?>(1,e)).ToArray();
                    if (weights != null)
                    {
                        for (var i = 0; i < elements.Length; i++)
                        {
                            var weight = weights[(string)elements[i].Value!];
                            elements[i] = new KeyValuePair<float, object?>(
                                (float)Math.Pow(ChooseStatement.Random.NextDouble(), 1 / Convert.ToSingle(weight)),
                                elements[i].Value);
                        }

                        Array.Sort(elements, (a,b) => -(a.Key.CompareTo(b.Key)));
                    }

                    var offset = weights==null?ChooseStatement.Random.Next(elements.Length):0;
                    for (var count = 0; count < elements.Length; count++)
                    {
                        var e = elements[(count + offset) % elements.Length].Value;
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