using static NDScript.NDScript;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class VariableReferenceTests
    {
        [TestMethod]
        public void ReadVariable()
        {
            Assert.AreEqual(4, RunExpression("a", new KeyValuePair<string, object?>("a", 4)));
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void UndefinedVariable()
        {
            Assert.AreEqual(4, RunExpression("b", new KeyValuePair<string, object?>("a", 4)));
        }
    }
}