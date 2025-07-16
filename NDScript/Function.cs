using static NDScript.NDScript;
using NDScript.Syntax;

namespace NDScript
{
    public abstract class Function(string name, bool isDeterministic)
    {
        public readonly string Name = name;
        public readonly bool IsDeterministic = isDeterministic;

        public abstract bool Call(object?[] arguments, State s, CallStack? stack, Continuation k);

        public override string ToString()
        {
            return $"({Name})";
        }
    }
}
