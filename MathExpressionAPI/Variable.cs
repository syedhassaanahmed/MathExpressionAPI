using System.Linq;

namespace MathExpressionAPI
{
    public class Variable
    {
        public string Name { get; }
        public double Value { get; set; }

        public Variable(string v)
        {
            if (!IsValidVariableName(v))
            {
                throw new MathException("Invalid variable name: " + v);
            }

            Name = v;
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool IsValidVariableName(string v)
        {
            return !string.IsNullOrEmpty(v) && char.IsLetter(v[0]) && v.All(char.IsLetterOrDigit);
        }
    }
}
