using System;

namespace MathExpressionAPI
{
  public class Operator
  {
    public string Symbol { get; private set; }
    public int Precedence { get; private set; }

    Func<double, double> Unary;
    Func<double, double, double> Binary;

    public bool IsBinary
    {
      get
      {
        return Binary != null;
      }
    }

    public Operator(string symbol, Func<double, double> f)
    {
      Symbol = symbol;
      Precedence = int.MaxValue;
      Unary = f;
    }

    public Operator(string symbol, int p, Func<double, double, double> f)
    {
      Symbol = symbol;
      Precedence = p;
      Binary = f;
    }

    public double Operate(double a, double? b = null)
    {
      if(IsBinary)
      {
        if(b == null)
        {
          throw new MathException("Binary operator '" + Symbol + "' requires exactly 2 operands.");
        }
        
        return Binary(a, b.Value);
      }
      else
      {
        return Unary(a);
      }
    }

    public override string ToString()
    {
      return Symbol;
    }
  }
}
