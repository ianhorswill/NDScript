using NDScript;
using NDScript.Syntax;
using Sprache;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class AssignmentExpressionTests
    {
        [TestMethod()]
        public void VariableAssignment()
        {
            var statement = Parser.Statement.Parse("a = 1;");
            var s = new State([new((StateElement)"a", 0)], null);
            Assert.IsTrue(statement.Execute(s, null!, (v, ns) =>
                                            {
                                                Assert.AreEqual(1, ns["a"]);
                                                return true;
                                            })
            );
        }

        [TestMethod]
        public void ArrayAssignment()
        {
            Assert.AreEqual(15,NDScript.NDScript.RunProgram("var a = [1, 2 ,3]; a[1] = 15; return a[1];"));
        }
    }
}