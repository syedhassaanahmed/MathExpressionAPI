using System;

namespace MathExpressionAPI
{
  class MathException : Exception
  {
    public MathException(string message) : base(message) { }
  }
}
