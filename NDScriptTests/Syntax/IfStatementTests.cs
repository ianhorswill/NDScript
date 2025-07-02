using static NDScript.NDScript;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class IfStatementTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            Assert.AreEqual(0, RunProgram("if (1 == 0) return 1; else return 0;"));
            Assert.AreEqual(1, RunProgram("if (1 != 0) return 1; else return 0;"));
            Assert.AreEqual(0, RunProgram("if (1 == 0) return 1; return 0;"));
        }
    }
}