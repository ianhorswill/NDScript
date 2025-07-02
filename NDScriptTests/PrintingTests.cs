namespace NDScriptTests
{
    [TestClass()]
    public class PrintingTests
    {

        [TestMethod()]
        public void PrintTest()
        {
            Assert.AreEqual("1020", NDScript.NDScript.ProgramOutput("print(10,20);"));
            Assert.AreEqual("Foo\n", NDScript.NDScript.ProgramOutput("printLine(\"Foo\");"));
        }
    }
}