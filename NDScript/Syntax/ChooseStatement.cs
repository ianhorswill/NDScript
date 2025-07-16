using System;

namespace NDScript.Syntax
{
    public class ChooseStatement(int sourceLine, Statement[] options, bool chooseFirst) 
        // ReSharper disable once CoVariantArrayConversion
        : Statement(sourceLine, options)
    {
        public static readonly Random Random = new();

        public readonly Statement[] Options = options;
        public readonly bool ChooseFirst = chooseFirst;

        public override bool Execute(State s, CallStack? stack, NDScript.Continuation r, NDScript.Continuation k)
        {
            var offset = ChooseFirst?0:Random.Next(Options.Length);
            for (var count = 0; count < Options.Length; count++)
            {
                var o = Options[ (count+offset) % Options.Length ];
                if (o.Execute(s, stack, r, k))
                    return true;
            }

            return false;
        }
    }
}