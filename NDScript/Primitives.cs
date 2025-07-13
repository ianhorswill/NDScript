using System.Collections;
using System.Collections.Generic;
using NDScript.Syntax;

namespace NDScript
{
    public static class Primitives
    {
        public static readonly List<PrimitiveBase> AllPrimitives = new();

        static Primitives()
        {
            Printing.MakePrimitives();
            Minimization.MakePrimitives();
            Grid.MakePrimitives();
            Sets.MakePrimitives();
            Collections.MakePrimitives();
        }
    }
}