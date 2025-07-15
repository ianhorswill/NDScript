using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NDScript
{
    [DebuggerDisplay("({X},{Y})")]
    public class Position : IComparable<Position>
    {
        private static readonly Dictionary<(int, int), Position> PositionTable = new();
        public readonly int X;
        public readonly int Y;

        private Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Position At(int x, int y) {
            if (!PositionTable.TryGetValue((x, y), out var position))
            {
                position = new Position(x,y);
                PositionTable[(x, y)] = position;
            }

            return position;
        }

        public int CompareTo(Position? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var xComparison = X.CompareTo(other.X);
            if (xComparison != 0) return xComparison;
            return Y.CompareTo(other.Y);
        }

        public Position Left => At(X - 1, Y);
        public Position Right => At(X + 1, Y);
        public Position Up => At(X, Y - 1);
        public Position Down => At(X, Y + 1);


    }
}
