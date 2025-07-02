using System.Collections.Generic;

namespace NDScript
{
    public static class Primitives
    {
        public static readonly List<PrimitiveBase> AllPrimitives = new();

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
        }
    }
}