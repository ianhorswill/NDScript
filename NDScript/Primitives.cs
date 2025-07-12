using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using NDScript.Syntax;

namespace NDScript
{
    public static class Primitives
    {
        public static readonly List<PrimitiveBase> AllPrimitives = new();

        private static GeneralPrimitive ChooseElement = new GeneralPrimitive(
            "chooseElement",
            (args, state, k) =>
            {
                ArgumentCountException.Check(1, args, ChooseElement);
                var collection = ArgumentTypeException.Cast<ICollection>(args[0], ChooseElement.Name, "collection");
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

        

        static Primitives()
        {
            new GeneralPrimitive("print", (args, s, k) =>
            {
                var state = s;
                foreach (var item in args)
                    state = Printing.Print(item, state);
                return k(null, state);
            });

            new GeneralPrimitive("printLine", (args, s, k) =>
            {
                var state = s;
                foreach (var item in args)
                    state = Printing.Print(item, state);
                return k(null, Printing.NewLine(state));
            });

            new StatefulDeterministicPrimitive<string, object?>(
                "printImage",
                (url, state) => (null, Printing.PrintImage(url, state)));

            new DeterministicPrimitive<int, int, Position>("position", (x, y) => Position.At(x, y));

            // Kluge: this forces the minimization primitives to be created.
            Minimization.RemoveBudgetConstraints();
            Grid.MakePrimitives();
        }
    }
}