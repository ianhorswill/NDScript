using System;
using NDScript.Syntax;

namespace NDScript
{
    public class ExecutionException(AstNode node, CallStack? stack, Exception innerException) : Exception("Exception in user code", innerException)
    {
        public readonly AstNode AstNode = node;
        public readonly CallStack? Stack = stack;
    }
}
