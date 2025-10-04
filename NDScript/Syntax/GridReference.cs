using System;
using System.Collections.Immutable;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class GridReference(int sourceLine, Expression array, Expression x, Expression y) : SettableExpression(sourceLine, [array, x, y])
    {
        public readonly Expression Array = array;
        public readonly Expression X = x;
        public readonly Expression Y = y;

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k)
        {
            return Array.Execute(s, stack, r, (array, ns) => 
                X.Execute(ns, stack, r, (x, ms) =>
                    Y.Execute(ms, stack, r, (y, fs) =>
            {
                var grid = ArgumentTypeException.Cast<Grid>(array,
                    "Argument to grid reference was not a grid", fs);
                var xi = ArgumentTypeException.Cast<int>(x, "X index of grid reference isn't an integer", fs);
                var yi = ArgumentTypeException.Cast<int>(y, "X index of grid reference isn't an integer", fs);
                return k(grid.GetCell(xi, yi, fs), fs);
            })));
        }

        public override bool Set(object? value, State s, CallStack? stack, Continuation r, Continuation k)
        {
            return Array.Execute(s, stack, r, (array, ns) => 
                X.Execute(ns, stack, r, (x, ms) =>
                    Y.Execute(ms, stack, r, (y, fs) =>
                    {
                        var grid = ArgumentTypeException.Cast<Grid>(array,
                            "Argument to grid reference was not a grid", fs);
                        var xi = ArgumentTypeException.Cast<int>(x, "X index of grid reference isn't an integer", fs);
                        var yi = ArgumentTypeException.Cast<int>(y, "X index of grid reference isn't an integer", fs);
                        return k(value, grid.SetCell(xi, yi, fs, value));
                    })));
        }
    }
}