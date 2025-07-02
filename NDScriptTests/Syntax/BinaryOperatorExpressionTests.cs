namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class BinaryOperatorExpressionTests
    {
        [TestMethod()]
        public void Sum()
        {
            Assert.AreEqual(2, NDScript.NDScript.RunExpression("1+1"));
        }

        [TestMethod]
        public void ComplexExpression()
        {
            Assert.AreEqual(4, NDScript.NDScript.RunExpression("2*(1+1)"));
        }
    }
}