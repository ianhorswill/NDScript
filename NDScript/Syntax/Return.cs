using System;
using System.Collections.Generic;
using System.Text;

namespace NDScript.Syntax
{
    public class Return(Expression value) : Expression([value])
    {
        public readonly Expression Value = value;

        public override bool Execute(State s, NDScript.Continuation r, NDScript.Continuation k)
            => Value.Execute(s, r, r);
    }
}
