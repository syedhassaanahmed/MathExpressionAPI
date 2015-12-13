using System;

namespace MathExpressionAPI
{
    public class Operator
    {
        public string Symbol { get; }
        public int Precedence { get; private set; }

        readonly Func<double, double> _unary;
        readonly Func<double, double, double> _binary;

        public bool IsBinary => _binary != null;

        public Operator(string symbol, Func<double, double> f)
        {
            Symbol = symbol;
            Precedence = int.MaxValue;
            _unary = f;
        }

        public Operator(string symbol, int p, Func<double, double, double> f)
        {
            Symbol = symbol;
            Precedence = p;
            _binary = f;
        }

        public double Operate(double a, double? b = null)
        {
            if (!IsBinary)
                return _unary(a);

            if (b == null)
            {
                throw new MathException("_binary operator '" + Symbol + "' requires exactly 2 operands.");
            }

            return _binary(a, b.Value);
        }

        public override string ToString()
        {
            return Symbol;
        }
    }
}
