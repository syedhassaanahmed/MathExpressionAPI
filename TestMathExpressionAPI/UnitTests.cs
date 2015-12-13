using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MathExpressionAPI;

namespace TestMathExpressionAPI
{
    [TestClass]
    public class UnitTests
    {
        readonly IDictionary<string, string> _expressionTests = new Dictionary<string, string>
        {
            {"A-B+C", "AB-C+"},
            {"A*B+C", "AB*C+"},
            {"A+B*C", "ABC*+"},
            {"A*(B+C)", "ABC+*"},
            {"A*B+C/D", "AB*CD/+"},
            {"A*B^C+D", "ABC^*D+"},
            {"A*(B+C)/D", "ABC+*D/"},
            {"A*(B+C/D)", "ABCD/+*"},
            {"A*(B+C*D)+E", "ABCD*+*E+"},
            {"3+4*5/6", "345*6/+"},
            {"(300+23)*(43-21)/(84+7)", "30023+4321-*847+/"},
            {"(4+8)*(6-5)/((3-2)*(2+2))", "48+65-*32-22+*/"},
        };

        [TestMethod]
        public void TestPostfix()
        {
            foreach (var expressionTest in _expressionTests)
            {
                Assert.AreEqual(expressionTest.Value, new Expression(expressionTest.Key).Postfix);
            }
        }
    }
}
