using System.Collections.Immutable;

namespace NDScript.Syntax
{
    public class ArrayExpression(Expression[] elements) : Expression(elements)
    {
        public readonly Expression[] Elements = elements;
        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
        {
            var objectId = State.CreateObject(this);

            var argIndex = 0;
            var actualElements = new object?[Elements.Length];

            bool ElementEvaluated(object? value, State newState)
            {
                actualElements[argIndex++] = value;
                if (argIndex < Elements.Length)
                    return Elements[argIndex].Execute(newState, r, ElementEvaluated);
                var array = actualElements.ToImmutableArray();
                return k(objectId, newState.Global.Create(objectId, array));
            }

            if (Elements.Length == 0)
            {
                var array = actualElements.ToImmutableArray();
                return k(objectId, s.Global.Create(objectId, array));
            }
            return Elements[0].Execute(s, r, ElementEvaluated);
        }
    }
}
