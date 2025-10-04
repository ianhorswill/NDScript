namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class MemberReferenceTests
    {
        [TestMethod()]
        public void Read()
        {
            Assert.AreEqual("1",NDScript.NDScript.ProgramOutput("var o = {a:1, b:2}; print(o.a);"));
        }

        public void Write()
        {
            Assert.AreEqual("7",NDScript.NDScript.ProgramOutput("var o = {a:1, b:2}; o.a = 7; print(o.a);"));
        }
    }
}