using System.Collections.Immutable;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class ArrayReferenceTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            Assert.AreEqual(3, NDScript.NDScript.RunExpression("([1,2,3, 4])[2]"));
        }

        [TestMethod()]
        public void ObjectRead()
        {
            Assert.AreEqual("1",NDScript.NDScript.ProgramOutput("var o = {a:1, b:2}; print(o[\"a\"]);"));
        }

        public void ObjectWrite()
        {
            Assert.AreEqual("7",NDScript.NDScript.ProgramOutput("var o = {a:1, b:2}; o[\"a\"] = 7; print(o.a);"));
        }
    }
}