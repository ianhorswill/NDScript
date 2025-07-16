namespace NDScript.Syntax
{
    public abstract class Statement(int sourceLine, AstNode[] children) : AstNode(sourceLine, children);
}
