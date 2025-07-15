using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static NDScript.NDScript;

namespace NDScript
{
    public class Relation(string name, IEnumerable<(object, object)> extension) : Function(name)
    {
        private readonly HashSet<(object, object)> _extension = [..extension];
        private Dictionary<object?, ImmutableHashSet<object?>>? leftImage, rightImage;
        public override bool Call(object?[] arguments, State s, CallStack? stack, Continuation k)
        {
            ArgumentCountException.Check(2, arguments, this);
            return k(_extension.Contains((arguments[0]!, arguments[1]!)), s);
        }

        public ImmutableHashSet<object?> LeftImage(ImmutableHashSet<object?> rightSet)
        {
            if (leftImage == null)
            {
                leftImage = new();
                foreach (var pair in _extension)
                {
                    var left = pair.Item1;
                    var right = pair.Item2;

                    if (!leftImage.TryGetValue(right, out var old))
                        old =ImmutableHashSet<object?>.Empty;
                    leftImage[right] = old.Add(left);
                }
            }

            var result = ImmutableHashSet<object?>.Empty;
            foreach (var e in rightSet)
                if (leftImage.TryGetValue(e!, out var eImage))
                    result = result.Union(eImage);
            return result;
        }

        public ImmutableHashSet<object?> RightImage(ImmutableHashSet<object?> leftSet)
        {
            if (rightImage == null)
            {
                rightImage = new();
                foreach (var pair in _extension)
                {
                    var left = pair.Item1;
                    var right = pair.Item2;

                    if (!rightImage.TryGetValue(left, out var old))
                        old =ImmutableHashSet<object?>.Empty;
                    rightImage[left] = old.Add(right);
                }
            }

            var result = ImmutableHashSet<object?>.Empty;
            foreach (var e in leftSet)
                if (rightImage.TryGetValue(e!, out var eImage))
                    result = result.Union(eImage);
            return result;
        }

        internal static void MakePrimitives()
        {
            // ReSharper disable ObjectCreationAsStatement
            new GeneralPrimitive("relation",
                (args, state, _, k) =>
                {
                    (object, object) DecodeArg(object? arg)
                    {
                        var l = Collections.ConvertToList(arg, state,
                            "Arguments to relation must be two-element arrays");
                        if (l.Count != 2)
                            throw new ArgumentException("Arguments to relation must be two-element arrays");
                        return (l[0], l[1]);
                    }

                    return k(new Relation("relation", args.Select(DecodeArg)), state);
                });

            new DeterministicPrimitive<Relation, ImmutableHashSet<object?>, ImmutableHashSet<object?>>("leftImage",
                (r, set) => r.LeftImage(set));

            new DeterministicPrimitive<Relation, ImmutableHashSet<object?>, ImmutableHashSet<object?>>("rightImage",
                (r, set) => r.RightImage(set));
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}
