MathExpressionAPI
=================

Mathematical expression parser

* Takes string input, parses and identifies Variables, Constants and Operators.
* Converts expression to Postfix.
* Evaluates the expression based on provided values (Default Variable value is Zero if not provided).
* New operators can be added to OperatorLibrary collection.
* Variable values are read from Properties of Anonymous Objects.
* Unit Tests for Postfix conversion are available.


Example

  var e = new Expression("(A+B)/C");
  double result = e.Evaluate(new { A = 4, B = 8, C = 3 });
  Answer: 4
