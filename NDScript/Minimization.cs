using System;
using System.Runtime.CompilerServices;

namespace NDScript
{
    public static class Minimization
    {
        /// <summary>
        /// How much more cost we can absorb before we fail.
        /// </summary>
        public static int Budget = int.MaxValue;

        public static bool InsufficientBudget;

        public static void RemoveBudgetConstraints() => Budget = int.MaxValue;

        public static GeneralPrimitive? Cost;

        public static GeneralPrimitive? Minimize;

        internal static void MakePrimitives()
        {
            Minimize = new GeneralPrimitive("minimize", true,
                (args, callSite, state, stack, k) =>
                {
                    ArgumentCountException.Check(1, args, Minimize!);
                    var function = ArgumentTypeException.Cast<Function>(args[0], Minimize!.Name, "function", callSite, stack);
                    var save = Budget;
                    Budget = 0;
                    InsufficientBudget = false;
                    var finalState = state;
                    object? result = null;
                    var succeeded = false;
                    while (!function.Call(
                               Array.Empty<object?>(), callSite,
                               state,
                               new CallStack(Minimize, args, stack, callSite),
                               (value, newState) =>
                               {
                                   succeeded = true;
                                   result = value;
                                   finalState = newState;
                                   return true;
                               })
                           && InsufficientBudget)
                    {
                        InsufficientBudget = false;
                        Budget++;
                    }

                    var done = succeeded && k(result, finalState);
                    if (!done)
                        Budget = save;
                    return done;
                });

            Cost = new GeneralPrimitive(
                "cost", false,
                (args, callSite, state, _, k) =>
                {
                    ArgumentCountException.Check(1, args, Cost!);
                    var cost = ArgumentTypeException.Cast<int>(args[0], Cost!.Name, "cost");
                    if (cost > Budget)
                    {
                        InsufficientBudget = true;
                        // fail
                        return false;
                    }

                    Budget -= cost;
                    if (!k(Budget, state))
                    {
                        // We're backtracking, so restore our budget
                        Budget += cost;
                        return false;
                    }

                    // Continuation succeeded
                    return true;
                });
        }
    }
}
