using NDScript.Syntax;
using NDScript;
using Sprache;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class ReturnTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            var statement = Parser.Statement.Parse("{ var a = 0; a = 1; return a; }");
            var s = State.Default;
            Assert.IsTrue(statement.Execute(s, 
                (v, ns) =>
                        {
                            Assert.AreEqual(1, v);
                            return true;
                        },
                null!)
            );
        }
    }
}