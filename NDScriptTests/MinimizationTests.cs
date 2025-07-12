using static NDScript.NDScript;

namespace NDScriptTests
{
    [TestClass()]
    public class MinimizationTests
    {
        [TestMethod()]
        public void NoCostSolution()
        {
            Assert.AreEqual("1", ProgramOutput("function foo() { return 1; } print(minimize(foo));"));
        }

        [TestMethod]
        public void AlwaysChoosesCheap()
        {
            for (var i = 0; i < 100; i++)
                Assert.AreEqual("cheap", ProgramOutput(
                    "function cheap() { cost(1); print(\"cheap\"); } function expensive() { cost(2); print(\"expensive\"); } function run() { choose cheap(); or expensive(); } minimize(run);"
                    ));
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void FailsImpossible()
        {
            // This should fail
            ProgramOutput("function die() { fail; } minimize(die);");
        }
    }
}