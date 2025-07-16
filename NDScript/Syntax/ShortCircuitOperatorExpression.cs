using static NDScript.NDScript;

namespace NDScript.Syntax
{
    public class ShortCircuitOperatorExpression(
        int sourceLine, 
        bool idempotentValue,
        Expression leftArgument,
        Expression rightArgument)
        : Expression(sourceLine, [leftArgument, rightArgument])
    {
        public readonly bool IdempotentValue = idempotentValue;
        public readonly Expression LeftArgument = leftArgument;
        public readonly Expression RightArgument = rightArgument;

        public string OperatorName => IdempotentValue ? "||" : "&&";

        public override bool Execute(State s, CallStack? stack, Continuation r, Continuation k) 
            => LeftArgument.Execute(s, stack, r,
                (left, newState) => 
                    IdempotentValue == ArgumentTypeException.Cast<bool>(left, OperatorName, "left") 
                        ? k(left, newState) 
                        : RightArgument.Execute(newState, stack, r, k));
    }
}
