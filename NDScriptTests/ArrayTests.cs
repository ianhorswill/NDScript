using static NDScript.NDScript;

namespace NDScriptTests
{
    [TestClass()]
    public class ArrayTests
    {
        [TestMethod]
        public void Contains()
        {
            Assert.AreEqual("true", ProgramOutput("print(contains(2, [1,2,3]));"));
            Assert.AreEqual("false", ProgramOutput("print(contains(0, [1,2,3]));"));
        }

        [TestMethod]
        public void IsEmpty()
        {
            Assert.AreEqual("true", ProgramOutput("print(isEmpty([ ]));"));
            Assert.AreEqual("false", ProgramOutput("print(isEmpty([1,2,3]));"));
        }

        [TestMethod]
        public void IsSingleton()
        {
            Assert.AreEqual("true", ProgramOutput("print(isSingleton([1]));"));
            Assert.AreEqual("false", ProgramOutput("print(isSingleton([1,2,3]));"));
        }

        [TestMethod]
        public void SingletonValue()
        {
            Assert.AreEqual("1", ProgramOutput("print(singletonValue([1]));"));
            Assert.AreEqual("false", ProgramOutput("choose print(isSingleton([1,2,3])); or print(false);"));
        }

        [TestMethod]
        public void ChooseElement()
        {
            var choose1 = 0;
            var choose2 = 0;
            var output = "";
            for (int i = 0; i < 100; i++)
                switch (output = ProgramOutput("print(chooseElement([1,2]));"))
                {
                    case "1":
                        choose1++;
                        break;

                    case "2":
                        choose2++;
                        break;

                    default:
                        throw new Exception($"Invalid program output: {output}");
                }
            Assert.IsTrue(choose1 > 30);
            Assert.IsTrue(choose2 > 30);
        }

        [TestMethod]
        public void WeightedChooseElement()
        {
            var chooseA = 0;
            var chooseB = 0;
            var output = "";
            for (int i = 0; i < 100; i++)
                switch (output = ProgramOutput("print(chooseElement([\"a\",\"b\"], {a: 1, b: 2}));"))
                {
                    case "a":
                        chooseA++;
                        break;

                    case "b":
                        chooseB++;
                        break;

                    default:
                        throw new Exception($"Invalid program output: {output}");
                }
            Assert.IsTrue(chooseA < 40);
            Assert.IsTrue(chooseB > 60);
        }

        [TestMethod]
        public void UpdateTest()
        {
            Assert.AreEqual("[1, 1, 3]",
                ProgramOutput("var a = [1, 2, 3]; a[1] = 1; print(a);"));
        }
    }
}