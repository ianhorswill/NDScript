namespace NDScript.Syntax
{
    public abstract class Expression(int sourceLine, AstNode[] children) : Statement(sourceLine, children);
}
