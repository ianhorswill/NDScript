
using NDScript.Syntax;
using Sprache;
using static NDScript.NDScript;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class FunctionCallTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            Assert.AreEqual(2, RunProgram("function f(x) { return x+1; }  return f(1);"));
        }

        public static void TestStatementParse(string code, string errorPrefix)
        {
            try
            {
                Parser.Statement.Parse(code);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.IsTrue(e.Message.StartsWith(errorPrefix));
            }
        }

        [TestMethod()]
        public void MissingParenTest()
        {
            TestStatementParse("baz(foo(intersection(g[p],setOf(foo, bar));",
                "Parsing failure: unexpected ';'; expected )");
            TestStatementParse("var z = baz(foo(intersection(g[p], setOf(foo, bar));",
                "Parsing failure: unexpected ';'; expected )");
        }
    }
}