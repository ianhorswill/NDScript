using Microsoft.VisualStudio.TestTools.UnitTesting;
using NDScript;
using NDScript.Syntax;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NDScriptTests.Syntax
{
    [TestClass()]
    public class FunctionDeclarationTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            var statement = Parser.Statement.Parse("function f(x) { return x+1; }");
            var s = State.Default;
            Assert.IsTrue(statement.Execute(s, null!, (v, ns) =>
                {
                    var func = ns["f"] as UserFunction;
                    Assert.IsInstanceOfType<UserFunction>(func);
                    Assert.AreEqual("f", func.Name);
                    CollectionAssert.AreEqual( new StateElement[] { StateElement.Intern("x") }, func.Arguments);
                    var body = func.Body as Block;
                    Assert.IsInstanceOfType<Block>(body);
                    var realBody = body.Statements;
                    Assert.AreEqual(1, realBody.Length);
                    var ret = realBody[0] as Return;
                    Assert.IsInstanceOfType<Return>(ret);
                    Assert.IsInstanceOfType<BinaryOperatorExpression>(ret.Value);
                    return true;
                })
            );
        }
    }
}