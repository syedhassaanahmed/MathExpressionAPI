using System;
using System.Collections.Generic;
using System.Linq;

namespace MathExpressionAPI
{
    public static class OperatorLibrary
    {
        public static readonly IDictionary<string, Operator> Operators;

        static OperatorLibrary()
        {
            Func<double, double> factorial = null;
            factorial = a => (a <= 1) ? 1 : a*factorial(a - 1);

            var listOfOperators = new List<Operator>()
            {
                new Operator("+", 20, (a, b) => a + b),
                new Operator("-", 30, (a, b) => a - b),
                new Operator("/", 40, (a, b) => a/b),
                new Operator("*", 50, (a, b) => a*b),
                new Operator("^", 60, Math.Pow),
                new Operator("%", 10, (a, b) => a%b),
                new Operator("log", Math.Log),
                new Operator("!", a => factorial(a)),
                new Operator("ln", a => Math.Log(a, Math.E)),
                new Operator("sin", a => Math.Sin(a*(Math.PI/180))),
                new Operator("cos", a => Math.Cos(a*(Math.PI/180))),
                new Operator("tan", a => Math.Tan(a*(Math.PI/180)))
            };

            Operators = listOfOperators.ToDictionary(x => x.Symbol);
        }
    }
}
