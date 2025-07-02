using NDScript.Syntax;
using NDScript;
using Sprache;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class BlockTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            var statement = Parser.Statement.Parse("{ var a = 0; a =1; }");
            var s = State.Default;
            Assert.IsTrue(statement.Execute(s, null!, (v, ns) =>
                {
                    Assert.AreEqual(1, ns["a"]);
                    return true;
                })
            );
        }
    }
}