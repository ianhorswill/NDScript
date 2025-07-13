using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NDScript
{
    public class State
    {
        private readonly ImmutableSortedDictionary<StateElement, object?> _data;
        private readonly State? _parent;

        public bool IsGlobal => _parent == null;

        /// <summary>
        /// The top-level parent of this State object.  This corresponds to the currently active global bindings.
        /// </summary>
        public State Global
        {
            get
            {
                var s = this;
                while (s._parent != null)
                    s = s._parent;
                return s;
            }
        }

        /// <summary>
        /// Return a new state identical to this one, where the global component has been replaced with newGlobal
        /// </summary>
        public State ReplaceGlobal(State newGlobal) 
            => (_parent == null) ? newGlobal : new State(_data, _parent.ReplaceGlobal(newGlobal));

        public State(IEnumerable<KeyValuePair<StateElement, object?>> bindings, State? _parent) 
            : this(ImmutableSortedDictionary.CreateRange<StateElement, object?>(bindings), _parent)
        { }

        private State(ImmutableSortedDictionary<StateElement, object?> data, State? parent)
        {
            _data = data;
            _parent = parent;
        }

        public static readonly State Default = 
            new(ImmutableSortedDictionary.CreateRange<StateElement, object?>(
                Primitives.AllPrimitives.Select(
                        p => new KeyValuePair<StateElement, object?>((StateElement)p.Name, p))
                    .Append(new KeyValuePair<StateElement, object?>(Printing.PrintState, ImmutableList<object>.Empty))),
                null);

        public object? this[StateElement e]
        {
            get
            {
                for (var s = this; s != null; s = s._parent)
                {
                    if (s._data.TryGetValue(e, out var result))
                        return result;
                }
                throw new Exception($"Undefined variable {e}");
            }
        }

        public object? ValueOrDefault(StateElement e, object? def)
        {
            for (var s = this; s != null; s = s._parent)
            {
                if (s._data.TryGetValue(e, out var result))
                    return result;
            }

            return def;
        }

        public object? this[string variableName] => this[(StateElement)variableName];

        /// <summary>
        /// Replaces the value for an existing binding for the state element,
        /// or if none exists, adds one in the global scope.
        /// </summary>
        /// <param name="e">Variable or other state element to set</param>
        /// <param name="value">New value</param>
        /// <returns>New state with the update</returns>
        internal State Set(StateElement e, object? value)
        {
            if (_data.ContainsKey(e) || _parent == null)
                return new State(_data.SetItem(e, value), _parent);
            return new State(_data, _parent.Set(e, value));
        }

        internal State SetGlobal(StateElement e, object? value) => ReplaceGlobal(Global.Set(e, value));

        /// <summary>
        /// Adds or updates a binding in the current scope for the specified variable or other state element
        /// </summary>
        /// <param name="e">Variable or other state element to create/update</param>
        /// <param name="value">New value</param>
        /// <returns>Revised state</returns>
        public State Create(StateElement e, object? value) => new State(_data.SetItem(e, value), _parent);

        public static StateElement CreateObject(object? descriptor) => new StateElement(descriptor);
    }
}
