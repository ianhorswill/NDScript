using System;
using System.Collections.Immutable;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class GridReference(Expression array, Expression x, Expression y) : SettableExpression([array, x, y])
    {
        public readonly Expression Array = array;
        public readonly Expression X = x;
        public readonly Expression Y = y;

        public override bool Execute(State s, Continuation r, Continuation k)
        {
            return Array.Execute(s, r, (array, ns) => 
                X.Execute(ns, r, (x, ms) =>
                    Y.Execute(ms, r, (y, fs) =>
            {
                var grid = ArgumentTypeException.Cast<Grid>(array,
                    "Argument to grid reference was not a grid");
                var xi = ArgumentTypeException.Cast<int>(x, "X index of grid reference isn't an integer");
                var yi = ArgumentTypeException.Cast<int>(y, "X index of grid reference isn't an integer");
                return k(grid.GetCell(xi, yi, fs), fs);
            })));
        }

        public override bool Set(object? value, State s, Continuation r, Continuation k)
        {
            return Array.Execute(s, r, (array, ns) => 
                X.Execute(ns, r, (x, ms) =>
                    Y.Execute(ms, r, (y, fs) =>
                    {
                        var grid = ArgumentTypeException.Cast<Grid>(array,
                            "Argument to grid reference was not a grid");
                        var xi = ArgumentTypeException.Cast<int>(x, "X index of grid reference isn't an integer");
                        var yi = ArgumentTypeException.Cast<int>(y, "X index of grid reference isn't an integer");
                        return k(value, grid.SetCell(xi, yi, fs, value));
                    })));
        }
    }
}