using static NDScript.NDScript;
using NDScript.Syntax;

namespace NDScript
{
    public abstract class Function(string name)
    {
        public readonly string Name = name;

        public abstract bool Call(object?[] arguments, State s, CallStack? stack, Continuation k);

        public override string ToString()
        {
            return $"({Name})";
        }
    }
}
