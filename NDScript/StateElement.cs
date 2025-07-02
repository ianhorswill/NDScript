using System;
using System.Collections.Generic;

namespace NDScript
{
    public sealed class StateElement : IComparable<StateElement>
    {
        private static uint _uidCounter;
        public readonly uint Uid = _uidCounter++;

        public readonly object? Description;

        public StateElement(object? description)
        {
            Description = description;
        }

        public int CompareTo(StateElement? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Uid.CompareTo(other.Uid);
        }

        private static readonly Dictionary<object, StateElement> Table = new();

        public static StateElement Intern(object o)
        {
            if (!Table.TryGetValue(o, out var e))
            {
                e = new StateElement(o);
                Table[o] = e;
            }

            return e;
        }

        public static explicit operator StateElement(string o) => Intern(o);

        public override string ToString() =>
            Description switch
            {
                string s => s,
                null => "(null)",
                Type t => $"({t.Name} {Uid})",
                _ => $"({Description} {Uid})"
            };
    }
}
