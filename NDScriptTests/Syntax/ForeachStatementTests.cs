using NDScript.Syntax;
using Sprache;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class ForeachStatementTests
    {
        [TestMethod]
        public void ParseTest()
        {
            Parser.ForeachStatement.Parse("foreach (row in map) { print(\"▒\"); foreach (tile in row) print(tile); printLine(\"▒\"); }");
        }
    }
}