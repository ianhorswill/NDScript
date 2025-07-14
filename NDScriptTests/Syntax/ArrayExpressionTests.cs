using System.Collections.Immutable;
using static NDScript.NDScript;

namespace NDScriptTests.Syntax
{
    [TestClass]
    public class ArrayExpressionTests
    {
        [TestMethod]
        public void ExecuteTest()
        {
            var value = (ImmutableArray<object>)RunExpression("[1,2,3, 4]")!;
            
            CollectionAssert.AreEqual(new object[] { 1, 2, 3, 4 }, value.ToArray());
        }

        [TestMethod]
        public void InitializerTest()
        {
            Assert.AreEqual(1, RunProgram("var a = [1, 2, 3]; return a[0];"));
        }

        [TestMethod]
        public void NestedTest()
        {
            Assert.AreEqual(3, RunProgram("var a = [ [1, 2], [3, 4]]; return a[1][0];"));
        }
    }
}