using System;
using NDScript.Syntax;

namespace NDScript
{
    public class ExecutionException(AstNode node, State state, CallStack? stack, Exception innerException) : Exception("Exception in user code", innerException)
    {
        public readonly AstNode AstNode = node;
        public readonly State State = state;
        public readonly CallStack? Stack = stack;
    }
}
