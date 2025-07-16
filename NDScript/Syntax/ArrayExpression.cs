using System.Collections.Immutable;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    // ReSharper disable once CoVariantArrayConversion
    public class ArrayExpression(int sourceLine, Expression[] elements) : Expression(sourceLine, elements)
    {
        public readonly Expression[] Elements = elements;
        public override bool Execute(State s, CallStack?  stack, Continuation r, Continuation k)
        {
            var objectId = State.CreateObject(this);

            var argIndex = 0;
            var actualElements = new object?[Elements.Length];

            bool ElementEvaluated(object? value, State newState)
            {
                actualElements[argIndex++] = value;
                if (argIndex < Elements.Length)
                    return Elements[argIndex].Execute(newState, stack, r, ElementEvaluated);
                var array = actualElements.ToImmutableArray();
                return k(objectId, newState.SetGlobal(objectId, array));
            }

            if (Elements.Length == 0)
            {
                var array = actualElements.ToImmutableArray();
                return k(objectId, s.SetGlobal(objectId, array));
            }
            return Elements[0].Execute(s, stack, r, ElementEvaluated);
        }
    }
}
