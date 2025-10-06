using System;
using System.Collections.Immutable;

namespace NDScript.Syntax;

public class MemberReference(int sourceLine, Expression expression, string memberName) 
    : SettableExpression(sourceLine, [expression])
{
    public readonly Expression ObjectExpression = expression;
    public readonly string MemberName = memberName;


    public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k)
    {
        return ObjectExpression.Execute(s, stack, r, (o, ns) =>
        {
            var obj = ArgumentTypeException.Cast<CompoundObject>(o, $"Value is not a compound object", s, this, stack);
            var (value, success) = obj.GetMember(MemberName, ns);
            if (!success)
                    throw new ExecutionException(this, ns, stack,
                        new MissingMemberException($"Object has no field named {MemberName}"));
            return k(value, ns);
        });
    }

    public override bool Set(object? value, State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k)
    {
        return ObjectExpression.Execute(s, stack, r, (o, ns) =>
        {
            var obj = ArgumentTypeException.Cast<CompoundObject>(o, $"Value is not a compound object", s, this, stack);
            var (value, success) = obj.GetMember(MemberName, ns);
            return k(value, obj.SetMember(MemberName, value, s));
        });
    }
}