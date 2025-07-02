namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class ChooseStatementTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            var s = NDScript.NDScript.StateAfterProgram(
                "choose first a=0; or a=1; or a=2; choose first b=0; or b=1; or b=2; if (a+b != 3) fail;");

            Assert.AreEqual(1, s["a"]);
            Assert.AreEqual(2, s["b"]);
        }
    }
}