using System.Collections.Generic;

namespace NDScript
{
    public abstract class PrimitiveBase : Function
    {
        public static readonly List<PrimitiveBase> AllPrimitiveFunctions = new();

        protected PrimitiveBase(string name) : base(name)
        {
            Primitives.AllPrimitives.Add(this);
        }
    }
}
