using NDScript;
using NDScript.Syntax;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class FailTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            Assert.IsFalse(new Fail().Execute(State.Default, (v,s)=> true, (v,s) => true));
        }
    }
}