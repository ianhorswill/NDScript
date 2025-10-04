using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    // ReSharper disable once CoVariantArrayConversion
    public class ObjectExpression(int sourceLine, KeyValuePair<string,Expression>[] fields) : Expression(sourceLine, fields.Select(f => f.Value).ToArray())
    {
        public readonly KeyValuePair<string,Expression>[] Fields = fields;
        public override bool Execute(State s, CallStack?  stack, Continuation r, Continuation k)
        {
            var argIndex = 0;
            var actualElements = new KeyValuePair<string,object?>[Fields.Length];

            bool ElementEvaluated(object? value, State newState)
            {
                actualElements[argIndex] = new KeyValuePair<string, object?>(Fields[argIndex].Key, value);
                argIndex++;
                if (argIndex < Fields.Length)
                    return Fields[argIndex].Value.Execute(newState, stack, r, ElementEvaluated);
                return k(new CompoundObject(actualElements.ToImmutableDictionary()), newState);
            }

            if (Fields.Length == 0)
            {
                return k(new CompoundObject(ImmutableDictionary<string,object?>.Empty), s);
            }
            return Fields[0].Value.Execute(s, stack, r, ElementEvaluated);
        }
    }
}