namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class ConstantTests
    {
        [TestMethod()]
        public void Integer()
        {
            Assert.AreEqual(9, NDScript.NDScript.RunExpression("9"));
            Assert.AreEqual(-9, NDScript.NDScript.RunExpression("-9"));
            Assert.AreEqual(9.0f, NDScript.NDScript.RunExpression("9.0"));
            Assert.AreEqual(-9.0f, NDScript.NDScript.RunExpression("-9.0"));
        }

        [TestMethod]
        public void String()
        {
            Assert.AreEqual("This is a test", NDScript.NDScript.RunExpression("\"This is a test\""));
        }

        [TestMethod]
        public void True()
        {
            Assert.AreEqual(true, NDScript.NDScript.RunExpression("true"));
        }

        [TestMethod]
        public void False()
        {
            Assert.AreEqual(false, NDScript.NDScript.RunExpression("false"));
        }

        [TestMethod]
        public void Null()
        {
            Assert.AreEqual(null, NDScript.NDScript.RunExpression("null"));
        }
    }
}