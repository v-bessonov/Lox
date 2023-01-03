using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Interpreter;


public class Interpreter : IVisitor<object>
{
    
    public void Interpret(Expression expression) {
        try {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        } catch (RuntimeError error) {
            Lox.RuntimeError(error);
        }
    }
    
    private string Stringify(object obj) {
        if (obj is null)
        {
            return "nil";
        }
        if (obj is double) {
            var text = obj.ToString();
            if (text.EndsWith(".0")) {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }
        return obj.ToString();
    }
    
    public object VisitLiteralExpression(Literal expression)
    {
        return expression.Value;
    }

    public object VisitUnaryExpression(Unary expression)
    {
        var right = Evaluate(expression.Right);
        switch (expression.Operation.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expression.Operation, right);
                return -(double)right;
        }

        // Unreachable.
        return null;
    }
    
    private bool IsTruthy(object obj) {
        if (obj == null)
        {
            return false;
        }
        if (obj is bool )
        {
            return (bool)obj;
        }
        return true;
    }

    public object VisitBinaryExpression(Binary expression)
    {
        var left = Evaluate(expression.Left);
        var right = Evaluate(expression.Right);
        switch (expression.Operation.Type)
        {
            case TokenType.GREATER:
                CheckNumberOperand(expression.Operation, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperand(expression.Operation, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperand(expression.Operation, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperand(expression.Operation, left, right);
                return (double)left <= (double)right;
            case TokenType.MINUS:
                CheckNumberOperand(expression.Operation, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                if (left is double && right is double)
                {
                    return (double)left + (double)right;
                }

                if (left is string && right is string)
                {
                    return (string)left + (string)right;
                }
                throw new RuntimeError(expression.Operation,
                    "Operands must be two numbers or two strings.");
                break;
            case TokenType.SLASH:
                CheckNumberOperand(expression.Operation, left, right);
                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperand(expression.Operation, left, right);
                return (double)left * (double)right;
            case TokenType.BANG_EQUAL:
            {
                return !IsEqual(left, right);
            }
            case TokenType.EQUAL_EQUAL:
            {
                return IsEqual(left, right);
            }
        }

        // Unreachable.
        return null;
    }
    
    private void CheckNumberOperand(Token operation, object operand) {
        if (operand is double)
        {
            return;
        }
        throw new RuntimeError(operation, "Operand must be a number.");
    }
    
    private void CheckNumberOperand(Token operation, object left, object right) {
        if (left is double && right is double)
        {
            return;
        }
        throw new RuntimeError(operation, "Operands must be a numbers.");
    }

    public object VisitGroupingExpression(Grouping expression) {
        return Evaluate(expression.GroupingExpression);
    }
    
    private object Evaluate(Expression expression) {
        return expression.Accept(this);
    }
    
    private bool IsEqual(object a, object b) {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null)
        {
            return false;
        }
        return a.Equals(b);
    }
}