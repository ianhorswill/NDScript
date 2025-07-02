
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
    }
}