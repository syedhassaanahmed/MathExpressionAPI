using System;
using System.Collections.Generic;
using System.Linq;

namespace MathExpressionAPI
{
    public class Expression
    {
        readonly List<object> _parsedExpression = new List<object>();
        readonly List<object> _postfix = new List<object>();

        public string Postfix => GetStringFromList(_postfix);

        IEnumerable<Variable> Variables => _parsedExpression.OfType<Variable>();

        public Expression(string expression)
        {
            if (string.IsNullOrEmpty(expression) || expression.Trim() == "")
            {
                throw new MathException("Expression can not be empty.");
            }

            //No spaces will make life simpler
            expression = expression.Replace(" ", "");

            //Parse expression string into series of identifiable objects (double, Variable, Operator etc)
            ParseExpression(expression);

            GeneratePostfix();
        }

        void ParseExpression(string expression)
        {
            var operatorIndices = GetOperatorIndices(expression);

            //This string will hold possible operands
            var operand = "";
            var i = 0;

            while (i < expression.Length)
            {
                if (expression[i] == '(')
                {
                    _parsedExpression.Add("(");
                }
                else if (operatorIndices.ContainsKey(i) || expression[i] == ')')
                {
                    //We encountered an operator or ')', anything before might be an operand
                    AddOperandIfPossible(operand);

                    //Reset operand search
                    operand = "";

                    if (expression[i] == ')')
                    {
                        _parsedExpression.Add(")");
                    }
                    else
                    {
                        var op = OperatorLibrary.Operators[operatorIndices[i]];
                        _parsedExpression.Add(op);

                        //Skip as many characters as operator symbol length
                        i += op.Symbol.Length;
                        continue;
                    }
                }
                else
                {
                    operand += expression[i];
                }

                i++;
            }

            //Add last token too
            AddOperandIfPossible(operand);
        }

        static SortedDictionary<int, string> GetOperatorIndices(string expression)
        {
            var result = new SortedDictionary<int, string>();

            foreach (var op in OperatorLibrary.Operators.Keys)
            {
                var occurence = expression.Split(new[] {op}, StringSplitOptions.None).Length - 1;
                var index = -1;

                for (var i = 0; i < occurence; i++)
                {
                    index = expression.IndexOf(op, index + 1, StringComparison.Ordinal);
                    result.Add(index, op);
                }
            }

            return result;
        }

        void AddOperandIfPossible(string token)
        {
            if (token == "")
            {
                return;
            }

            object operand;

            double d;
            if (double.TryParse(token, out d))
            {
                operand = d;
            }
            else if (Variable.IsValidVariableName(token))
            {
                operand = new Variable(token);
            }
            else
            {
                throw new MathException("Invalid operand found: " + token);
            }

            _parsedExpression.Add(operand);
        }

        public override string ToString()
        {
            return GetStringFromList(_parsedExpression);
        }

        static string GetStringFromList(IEnumerable<object> list)
        {
            return string.Join("", list.Select(x => x.ToString()));
        }

        void GeneratePostfix()
        {
            var stack = new Stack<object>();

            foreach (var o in _parsedExpression)
            {
                if (o is double || o is Variable)
                {
                    _postfix.Add(o);
                }
                else if (o is Operator)
                {
                    var op = o as Operator;

                    while (stack.Any())
                    {
                        var top = stack.Peek();

                        //Until we find ( or operator with lower precedence on top of stack, we keep popping
                        if (top.ToString() == "(" || (top is Operator && (top as Operator).Precedence < op.Precedence))
                        {
                            break;
                        }
                        _postfix.Add(stack.Pop());
                    }

                    stack.Push(op);
                }
                else switch (o.ToString())
                {
                    case "(":
                        stack.Push(o);
                        break;
                    case ")":
                        //Pop until we find the matching (, also remove ( from stack
                        while (stack.Any())
                        {
                            var top = stack.Pop();

                            if (top.ToString() == "(")
                            {
                                break;
                            }

                            _postfix.Add(top);
                        }
                        break;
                }
            }

            //Pop until empty
            while (stack.Any())
            {
                _postfix.Add(stack.Pop());
            }
        }

        public double Evaluate(object varValues = null)
        {
            //Set values of variables
            SetVariableValues(varValues);

            var stack = new Stack<double>();

            foreach (var o in _postfix)
            {
                if (o is double)
                {
                    stack.Push((double) o);
                }
                else if (o is Variable)
                {
                    stack.Push((o as Variable).Value);
                }
                else if (o is Operator)
                {
                    var op = o as Operator;
                    var b = stack.Pop();

                    //if binary, pop once more, otherwise use unary overload and push result back to stack
                    stack.Push(op.IsBinary ? op.Operate(stack.Pop(), b) : op.Operate(b));
                }
            }

            return stack.Pop();
        }

        void SetVariableValues(object varValues)
        {
            var listOfVariables = Variables.ToList();

            if (varValues == null || !listOfVariables.Any())
                return;

            var properties = varValues.GetType().GetProperties();

            if (properties.Length <= 0)
                return;

            foreach (var v in listOfVariables)
            {
                //Find property with same variable name
                var property = properties.SingleOrDefault(x => x.Name == v.Name);

                if (property == null)
                    continue;

                var propertyValue = property.GetValue(varValues);

                if (propertyValue == null)
                {
                    throw new MathException("Value of '" + v.Name + "' can not be null.");
                }

                double doubleValue;
                //If variable is found, it must have proper value
                if (!double.TryParse(propertyValue.ToString(), out doubleValue))
                {
                    throw new MathException(
                        $"Incorrect value '{propertyValue}' of type '{propertyValue.GetType().Name}' for variable '{v.Name}'");
                }

                v.Value = doubleValue;
            }
        }
    }
}
