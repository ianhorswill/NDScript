using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NDScript
{
    public class CompoundObject(ImmutableDictionary<string, object?> data)
    {
        public ImmutableDictionary<string, object?> InitialValue = data;
        public StateElement? StateElement;

        public ImmutableDictionary<string, object?> CurrentDictionary(State state)
        {
            var dict = InitialValue;
            if (StateElement != null)
            {
                var d = state.ValueOrDefault(StateElement, null);
                if (d != null)
                    dict = (ImmutableDictionary<string, object?>)d;
            }

            return dict;
        }

        public (object?, bool found) GetMember(string memberName, State state) => 
            CurrentDictionary(state).TryGetValue(memberName, out var value) 
                ? (value, true) 
                : (null, false);

        public State SetMember(string memberName, object? newValue, State state)
        {
            var dict = CurrentDictionary(state);
            if (StateElement == null)
                StateElement = new StateElement(this);

            return state.SetGlobal(StateElement, dict.SetItem(memberName, newValue));
        }

        public static void MakePrimitives()
        {
            new DeterministicPrimitive<CompoundObject, object?[]>("nonsingletonFieldsOf", 
                (site, stack, s, o) =>
                    o.CurrentDictionary(s).Where(pair => !Collections.IsSingleton(
                            ArgumentTypeException.Cast<ICollection<object?>>(pair.Value!, 
                                "In call to nonsingletonFieldsOf, field of object was not a collection", s,null, stack)))
                    .Select(p => p.Key).Cast<object?>().ToArray());
        }
    }
}
