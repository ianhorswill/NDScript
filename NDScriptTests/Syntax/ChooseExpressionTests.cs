namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class ChooseExpressionTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            var s = NDScript.NDScript.StateAfterProgram(
                "var a = choose(0,1,2); var b = choose(0,1,2); if (a+b != 3) fail;");

            Assert.AreEqual(1, s["a"]);
            Assert.AreEqual(2, s["b"]);
        }
    }
}