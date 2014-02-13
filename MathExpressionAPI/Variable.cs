using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionAPI
{
  public class Variable
  {
    public string Name { get; private set; }
    public double Value { get; set; }

    public Variable(string v)
    {
      if (!IsValidVariableName(v))
      {
        throw new MathException("Invalid variable name: " + v);
      }

      Name = v;
    }

    public Variable(string v, double d) : this(v)
    {
      Value = d;
    }

    public override string ToString()
    {
      return Name;
    }

    public static bool IsValidVariableName(string v)
    {
      return !string.IsNullOrEmpty(v) && char.IsLetter(v[0]) && v.All(c => char.IsLetterOrDigit(c));
    }
  }
}
