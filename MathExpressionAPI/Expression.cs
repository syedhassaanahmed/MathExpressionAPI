using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MathExpressionAPI
{
  public class Expression
  {
    private List<object> parsedExpression = new List<object>();
    private List<object> postfix = new List<object>();

    public string Postfix
    {
      get
      {
        return GetStringFromList(postfix);
      }
    }

    public IEnumerable<Variable> Variables
    {
      get
      {
        return FilterByType<Variable>();
      }
    }

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

    private void ParseExpression(string expression)
    {
      var operatorIndices = GetOperatorIndices(expression);

      //This string will hold possible operands
      string operand = "";
      int i = 0;
      
      while (i < expression.Length)
      {
        if (expression[i] == '(')
        {
          parsedExpression.Add("(");
        }
        else if (operatorIndices.ContainsKey(i) || expression[i] == ')')
        {
          //We encountered an operator or ')', anything before might be an operand
          AddOperandIfPossible(operand);

          //Reset operand search
          operand = "";

          if (expression[i] == ')')
          {
            parsedExpression.Add(")");
          }
          else
          {
            var op = OperatorLibrary.Operators[operatorIndices[i]];
            parsedExpression.Add(op);

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

    private SortedDictionary<int, string> GetOperatorIndices(string expression)
    {
      var result = new SortedDictionary<int, string>();

      foreach (var op in OperatorLibrary.Operators.Keys)
      {
        int occurence = expression.Split(new string[] { op }, StringSplitOptions.None).Length - 1;
        int index = -1;

        for (int i = 0; i < occurence; i++)
        {
          index = expression.IndexOf(op, index + 1);
          result.Add(index, op);          
        }
      }

      return result;
    }

    private void AddOperandIfPossible(string token)
    {
      if(token == "")
      {
        return;
      }
      
      object operand = null;
      
      double d = 0;
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

      parsedExpression.Add(operand);
    }

    private IEnumerable<T> FilterByType<T>()
    {
      return parsedExpression.Where(v => v is T).Select(v => (T)v);
    }

    public override string ToString()
    {
      return GetStringFromList(parsedExpression);
    }

    private string GetStringFromList(List<object> list)
    {
      return string.Join("", list.Select(x => x.ToString()));
    }

    private void GeneratePostfix()
    {
      var stack = new Stack<object>();
      
      foreach(var o in parsedExpression)
      {
        if(o is double || o is Variable)
        {
          postfix.Add(o);
        }
        else if(o is Operator)
        {
          var op = o as Operator;

          while (stack.Any())
          {
            object top = stack.Peek();

            //Until we find ( or operator with lower precedence on top of stack, we keep popping
            if (top.ToString() == "(" || (top is Operator && (top as Operator).Precedence < op.Precedence))
            {
              break;
            }
            postfix.Add(stack.Pop());
          }

          stack.Push(op);
        }
        else if (o.ToString() == "(")
        {
          stack.Push(o);
        }
        else if(o.ToString() == ")")
        {
          //Pop until we find the matching (, also remove ( from stack
          while (stack.Any())
          {
            var top = stack.Pop();

            if (top.ToString() == "(")
            {
              break;
            }
            else
            {
              postfix.Add(top);
            }
          }
        }
      }

      //Pop until empty
      while (stack.Any())
      {
        postfix.Add(stack.Pop());
      }
    }

    public double Evaluate(object varValues = null)
    {
      //Set values of variables
      SetVariableValues(varValues);

      var stack = new Stack<double>();

      foreach (var o in postfix)
      {
        if (o is double)
        {
          stack.Push((double)o);
        }
        else if (o is Variable)
        {
          stack.Push((o as Variable).Value);
        }
        else if (o is Operator)
        {
          var op = o as Operator;
          double b = stack.Pop();

          //if binary, pop once more, otherwise use unary overload and push result back to stack
          stack.Push(op.IsBinary ? op.Operate(stack.Pop(), b) : op.Operate(b));
        }
      }

      return stack.Pop();
    }

    private void SetVariableValues(object varValues)
    {
      var listOfVariables = this.Variables;

      if (varValues != null && listOfVariables.Count() > 0)
      {
        var properties = varValues.GetType().GetProperties();

        if(properties != null && properties.Length > 0)
        {
          foreach(var v in listOfVariables)
          {
            //Find property with same variable name
            var property = properties.SingleOrDefault(x => x.Name == v.Name);            
            
            if(property != null)
            {
              object propertyValue = property.GetValue(varValues);              

              if (propertyValue == null)
              {
                throw new MathException("Value of '" + v.Name + "' can not be null.");
              }

              double doubleValue = 0;
              //If variable is found, it must have proper value
              if (!double.TryParse(propertyValue.ToString(), out doubleValue))
              {
                throw new MathException(string.Format("Incorrect value '{0}' of type '{1}' for variable '{2}'", 
                  propertyValue, propertyValue.GetType().Name, v.Name));
              }
              
              v.Value = doubleValue;
            }
          }
        }
      }
    }
  }
}
