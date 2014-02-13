using MathExpressionAPI;
using System;

namespace MathExpressionAPIConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      var e = new Expression("(A+B)/C");
      double result = e.Evaluate(new { A = 4, B = 8, C = 3 });
      Console.WriteLine(result);
      Console.ReadKey();
    }
  }
}
