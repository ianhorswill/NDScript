using Microsoft.VisualStudio.TestTools.UnitTesting;
using NDScript;
using static NDScript.NDScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDScriptTests
{
    [TestClass()]
    public class SetsTests
    {
        [TestMethod]
        public void SetOf()
        {
            Assert.AreEqual("{1, 2, 3}", ProgramOutput("print(setOf(1,2,3));"));
        }

        [TestMethod]
        public void Contains()
        {
            Assert.AreEqual("true", ProgramOutput("print(contains(2, setOf(1,2,3)));"));
            Assert.AreEqual("false", ProgramOutput("print(contains(0, setOf(1,2,3)));"));
        }

        [TestMethod]
        public void IsEmpty()
        {
            Assert.AreEqual("true", ProgramOutput("print(isEmpty(setOf()));"));
            Assert.AreEqual("false", ProgramOutput("print(isEmpty(setOf(1,2,3)));"));
        }

        [TestMethod]
        public void IsSingleton()
        {
            Assert.AreEqual("true", ProgramOutput("print(isSingleton(setOf(1)));"));
            Assert.AreEqual("false", ProgramOutput("print(isSingleton(setOf(1,2,3)));"));
        }

        [TestMethod]
        public void SingletonValue()
        {
            Assert.AreEqual("1", ProgramOutput("print(singletonValue(setOf(1)));"));
            Assert.AreEqual("false", ProgramOutput("choose print(isSingleton(setOf(1,2,3))); or print(false);"));
        }

        [TestMethod]
        public void Union()
        {
            Assert.AreEqual("{1, 2, 3}", ProgramOutput("print(union(setOf(1), setOf(2,3)));"));
        }

        [TestMethod]
        public void Intersection()
        {
            Assert.AreEqual("{2}", ProgramOutput("print(intersection(setOf(1,2), setOf(2,3)));"));
            Assert.AreEqual("{}", ProgramOutput("print(intersection(setOf(1), setOf(2,3)));"));
        }
    }
}