using System.Collections.Generic;

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
            Queue.MakePrimitives();
            Collections.MakePrimitives();
            Relation.MakePrimitives();
            Logic.MakePrimitives();
        }
    }
}