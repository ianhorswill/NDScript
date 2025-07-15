using System.Collections.Generic;

namespace NDScript.Syntax
{
    public class ForeachStatement(string variable, Expression collection, Statement body) : Expression([collection, body])
    {
        public readonly StateElement Variable = StateElement.Intern(variable);
        public readonly Expression Collection = collection;
        public readonly Statement Body = body;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k)
        {
            return Collection.Execute(s, stack, r, (c, newState) =>
            {
                var collection = Collections.ConvertToList(
                    c, newState,
                    "Collection expression in a foreach was not a collection type");
                var i = 0;

                bool NextElement(object? _, State state) =>
                    i == collection.Count
                        ? k(null, state)
                        : Body.Execute(
                            new State([new KeyValuePair<StateElement, object?>(Variable, collection[i++])], state),
                            stack,
                            r, NextElement);

                return NextElement(null, newState);
            });
        }
    }
}
