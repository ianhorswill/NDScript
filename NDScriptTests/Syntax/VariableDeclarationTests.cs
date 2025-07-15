using Microsoft.VisualStudio.TestTools.UnitTesting;
using NDScript.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDScript;
using Sprache;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class VariableDeclarationTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            var statement = Parser.Statement.Parse("var a = 1;");
            var s = State.Default;
            Assert.IsTrue(statement.Execute(s, null, null!, (v, ns) =>
                {
                    Assert.AreEqual(1, ns["a"]);
                    return true;
                })
            );
        }
    }
}