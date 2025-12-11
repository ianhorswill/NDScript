using System.Collections.Generic;

namespace NDScript
{
    public static class Primitives
    {
        public static readonly List<PrimitiveBase> AllPrimitives = new();

        static Primitives()
        {
            new DeterministicPrimitive<int, int, int>("floor", (x, y) => x / y);
            Printing.MakePrimitives();
            Minimization.MakePrimitives();
            Grid.MakePrimitives();
            Sets.MakePrimitives();
            Queue.MakePrimitives();
            Collections.MakePrimitives();
            Relation.MakePrimitives();
            Logic.MakePrimitives();
            CompoundObject.MakePrimitives();
        }
    }
}