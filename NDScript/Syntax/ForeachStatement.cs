using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NDScript.Syntax
{
    public class ForeachStatement(string variable, Expression collection, Statement body) : Expression([collection, body])
    {
        public readonly StateElement Variable = StateElement.Intern(variable);
        public readonly Expression Collection = collection;
        public readonly Statement Body = body;

        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
        {
            return Collection.Execute(s, r, (c, newState) =>
            {
                var objectId = ArgumentTypeException.CastObject<ArrayExpression>(c, typeof(Array),
                    "Collection expression in a foreach was not a collection type");
                var collection = (ImmutableArray<object?>)newState[objectId]!;
                var i = 0;

                bool NextElement(object? _, State state) =>
                    i == collection.Length
                        ? k(null, state)
                        : Body.Execute(
                            new State([new KeyValuePair<StateElement, object?>(Variable, collection[i++])], state), r, NextElement);

                return NextElement(null, newState);
            });
        }
    }
}
