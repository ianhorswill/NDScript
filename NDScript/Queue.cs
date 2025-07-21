using System;
using System.Collections.Immutable;
using System.Linq;
using NDScript.Syntax;

namespace NDScript
{
    public class Queue
    {
        public readonly StateElement Contents = new(typeof(Queue));

        public ImmutableQueue<object?> CurrentContents(State s) =>
            (ImmutableQueue<object?>)s.Global.ValueOrDefault(Contents,
                ImmutableQueue<object?>.Empty)!;

        public State Enqueue(State s, object? newValue)
        {
            var current = CurrentContents(s);
            return current.Contains(newValue) ? s : s.SetGlobal(Contents, current.Enqueue(newValue));
        }

        public (object? value, State newState) Dequeue(State s, AstNode site, CallStack stack)
        {
            var current = CurrentContents(s);
            if (current.IsEmpty)
                throw new ExecutionException(site, stack, new InvalidOperationException("Dequeue from empty queue"));
            var newQueue = current.Dequeue(out var next);
            return (next, s.SetGlobal(Contents, newQueue));
        }

        private Queue()
        { }

        #region Primitives
        internal static void MakePrimitives()
        {
            // ReSharper disable ObjectCreationAsStatement
            new DeterministicPrimitive<Queue>("queue", (_, _, _) => new Queue());

            new StatefulDeterministicPrimitive<Queue, object?>("dequeue",
                (site, stack, queue, state) => queue.Dequeue(state, site, stack));
            new StatefulDeterministicPrimitive<object?, Queue, object?>("enqueue",
                (_, _, newValue, queue, state) =>
                    (newValue, queue.Enqueue(state, newValue)));
            // ReSharper restore ObjectCreationAsStatement
        }
        #endregion
    }
}
