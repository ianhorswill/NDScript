using NDScript;
using static NDScript.NDScript;

namespace NDScriptTests
{
    [TestClass()]
    public class QueueTests
    {

        [TestMethod()]
        public void EnqueueDequeueTest()
        {
            Assert.AreEqual("a\nb\nc\n", ProgramOutput(@"// Enter program here.
var q = queue();
enqueue(""a"", q);
enqueue(""b"", q);
enqueue(""c"", q);
printLine(dequeue(q));
printLine(dequeue(q));
printLine(dequeue(q));"));
        }

        [TestMethod(), ExpectedException(typeof(InvalidOperationException))]
        public void ErrorOnEmptyTest()
        {
            try
            {
                ProgramOutput("dequeue(queue());");
            }
            catch (ExecutionException e)
            {
                throw e.InnerException!;
            }
        }
    }
}