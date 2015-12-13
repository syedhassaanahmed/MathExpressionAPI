using MathExpressionAPI;
using System;

namespace MathExpressionAPIConsole
{
    static class Program
    {
        static void Main()
        {
            var e = new Expression("(A+B)/C");
            var result = e.Evaluate(new {A = 4, B = 8, C = 3});
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
