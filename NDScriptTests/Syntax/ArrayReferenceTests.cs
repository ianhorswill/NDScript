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
    }
}