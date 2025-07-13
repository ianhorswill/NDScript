using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using NDScript.Syntax;

namespace NDScript
{
    public class Grid
    {
        public readonly int Width;
        public readonly int Height;
        public readonly StateElement Contents = new StateElement(typeof(Grid));

        public ImmutableSortedDictionary<Position, object?> CurrentContents(State s) =>
            (ImmutableSortedDictionary<Position, object?>)s.Global.ValueOrDefault(Contents,
                ImmutableSortedDictionary<Position, object?>.Empty)!;

        public object? GetCell(Position p, State s)
        {
            var current = CurrentContents(s);
            return current[p];
        }

        public object? GetCell(int x, int y, State s) => GetCell(Position.At(x, y), s);

        public State SetCell(Position p, State s, object? newValue)
        {
            var current = CurrentContents(s);
            return s.SetGlobal(Contents, current.SetItem(p, newValue));
        }

        public State SetCell(int x, int y, State s, object? newValue) => SetCell(Position.At(x, y), s, newValue);

        private Grid(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static (Grid grid, State state) MakeGrid(State s, ImmutableArray<object?> rows)
        {
            ImmutableArray<object?> Row(int i)
            {
                // Rows is an array of arrays, but references to arrays in the global state are really
                // StateElement objects.  We have to look up the value of that state element to get the
                // actual array representing a given row.
                var si = ArgumentTypeException.CastObject<ArrayExpression>(rows[i], typeof(Array),
                    "Row initializer for grid was not an array.");
                return (ImmutableArray<object?>)s.Global[si]!;
            }

            var width = Row(0).Length;
            var height = rows.Length;

            var grid = new Grid(width, height);

            var initialContents = new List<KeyValuePair<Position, object?>>();

            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    initialContents.Add(new KeyValuePair<Position, object?>(Position.At(x,y), Row(y)[x]));

            return (grid, s.SetGlobal(grid.Contents, initialContents.ToImmutableSortedDictionary()));
        }

        public bool ValidPosition(Position p) =>
            p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height;

        public IEnumerable<Position> NeighborsOf(Position p, State s)
        {
            var neighbor = p.Left;
            if (ValidPosition(neighbor)) yield return neighbor;
            neighbor = p.Right;
            if (ValidPosition(neighbor)) yield return neighbor;
            neighbor = p.Up;
            if (ValidPosition(neighbor)) yield return neighbor;
            neighbor = p.Down;
            if (ValidPosition(neighbor)) yield return neighbor;
        }

        #region Primitives

        private static GeneralPrimitive? constructor;

        internal static void MakePrimitives()
        {
            constructor = new GeneralPrimitive("grid",
                (args, s, k) =>
                {
                    ArgumentCountException.Check(1, args, constructor!);
                    var si = ArgumentTypeException.CastObject<ArrayExpression>(args[0], typeof(Array),
                        "Initializer for grid should be an array of arrays");
                    var gridInfo = MakeGrid(s, (ImmutableArray<object?>)s[si]!);
                    return k(gridInfo.grid, gridInfo.state);
                });

            new StatefulDeterministicPrimitive<Grid, Grid>(
                "printGrid",
                (grid, state) =>
                {
                    var b = new StringBuilder();
                    if (Printing.HtmlOutput)
                        b.Append("<table>");
                    for (var y = 0; y < grid.Height; y++)
                    {
                        if (Printing.HtmlOutput)
                            b.Append("<tr>");
                        for (var x = 0; x < grid.Width; x++)
                        {
                            if (Printing.HtmlOutput)
                                b.Append("<td>");
                            b.Append(grid.GetCell(x, y, state));
                            if (Printing.HtmlOutput)
                                b.Append("</td>");
                        }
                        if (Printing.HtmlOutput)
                            b.Append("</tr>");
                        else 
                            b.Append('\n');
                    }
                    if (Printing.HtmlOutput)
                        b.Append("</table>");
                    return (grid, Printing.PrintRaw(b.ToString(), state));
                });

            new StatefulDeterministicPrimitive<Grid, Grid>(
                "printTilemap",
                (grid, state) =>
                {
                    if (!Printing.HtmlOutput)
                        throw new InvalidOperationException($"Can't output tilemap when not rendering HTML");

                    var b = new StringBuilder();
                    b.Append("<p>");
                    for (var y = 0; y < grid.Height; y++)
                    {
                        for (var x = 0; x < grid.Width; x++) 
                            b.Append($"<img src=\"{Printing.AddImageExtensionIfNecessary((string)grid.GetCell(x, y, state))}\">");
                        b.Append("<br>");
                    }
                    b.Append("</p>");
                    return (grid, Printing.PrintRaw(b.ToString(), state));
                });

            new DeterministicPrimitive<Grid, IEnumerable<object?>>("positionsOf", (s, g)
                => g.CurrentContents(s).Select(p => p.Key));

            new DeterministicPrimitive<Grid, IEnumerable<object?>>("nonsingletonPositionsOf", (s, g)
                => g.CurrentContents(s).Where(p => Collections.IsSingleton((ICollection<object?>)p.Value!)).Select(p => p.Key));

            new DeterministicPrimitive<Position, Grid, IEnumerable<Position>>("neighborsOf", (s, p, g) => g.NeighborsOf(p, s));
        }


        #endregion
    }
}
