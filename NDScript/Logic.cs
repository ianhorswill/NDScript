using System.Collections.Generic;

namespace NDScript;

public static class Logic
{
    public static bool IsNegated(string literal, out string unnegated)
    {
        unnegated = literal;
        if (literal.Length > 0 && literal[0] == '!')
        {
            unnegated = literal.Substring(1);
            return true;
        }
        return false;
    }

    public static void MakePrimitives()
    {
        new DeterministicPrimitive<string, ICollection<object?>, bool>(
            "trueIn",
            (literal, model) =>
            {
                if (IsNegated(literal, out var unnegated))
                    return !model.Contains(unnegated);
                return model.Contains(literal);
            });
    }
}