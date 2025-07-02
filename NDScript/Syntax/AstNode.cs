using System;
using System.Collections.Generic;
using static NDScript.NDScript;

namespace NDScript.Syntax
{
    /// <summary>
    /// Base class for nodes in the abstract syntax tree (i.e. parse tree) of the program being executed.
    /// </summary>
    public abstract class AstNode
    {
        public abstract bool Execute(State s, Continuation r, Continuation k);
        public readonly AstNode[] Children;

        protected AstNode(AstNode[] children)
        {
            Children = children;
        }

        public void Walk<T>(Action<T> thunk) where T : AstNode
        {
            if (this is T t)
                thunk(t);
            foreach (var c in Children)
                Walk(thunk);
        }
    }
}
