using Lox.Interpreter.Native;
using Lox.Parser.Ast.Exceptions;
using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Statements;
using Lox.Scanner;

namespace Lox.Interpreter;


public class Interpreter : IExpressionVisitor<object>, IStatementVisitor
{
    private static readonly Environment _globals = new();
    
    private Environment _environment = _globals;
    public Interpreter()
    {
        _globals.Define("clock", new Clock());
    }
    public void Interpret(List<Statement> statements) {
        try {
            foreach (var statement in statements)
            {
                Execute(statement); 
            }
        } catch (RuntimeError error) {
            Lox.RuntimeError(error);
        }
    }
    public void Interpret(Expression expression) {
        try {
            var value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        } catch (RuntimeError error) {
            Lox.RuntimeError(error);
        }
    }
    
    private void Execute(Statement statement) {
        statement.Accept(this);
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

    public object VisitVariableExpression(Variable expression)
    {
        return _environment.Get(expression.Token);
    }

    public object VisitAssignmentExpression(Assignment expression)
    {
        var value = Evaluate(expression.Expression);
        _environment.Assign(expression.Token, value);
        return value;
    }

    public object VisitLogicalExpression(Logical expression)
    {
        var left = Evaluate(expression.Left);
        if (expression.Operation.Type == TokenType.OR) {
            if (IsTruthy(left))
            {
                return left;
            }
        } else {
            if (!IsTruthy(left))
            {
                return left;
            }
        }
        return Evaluate(expression.Right);
    }

    public object VisitCallExpression(Call expression)
    {
        var callee = Evaluate(expression.Callee);
        var arguments = new List<object>();
        foreach (var argument in expression.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }
        
        if (!(callee is ILoxCallable function)) {
            throw new RuntimeError(expression.Parenthesis, "Can only call functions and classes.");
        }

        if (arguments.Count != function.Arity) {
            throw new RuntimeError(expression.Parenthesis,
                $"Expected {function.Arity} arguments but got {arguments.Count}.");
        }
        
        return function.Call(this, arguments);
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

    public void VisitPrintStatement(PrintStatement statement)
    {
        var value = Evaluate(statement.Expression);
        Console.WriteLine(Stringify(value));
    }

    public void VisitExpressionStatement(ExpressionStatement statement)
    {
        Evaluate(statement.Expression);
    }

    public void VisitVariableDeclarationStatement(VariableDeclarationStatement statement)
    {
        object value = null;
        if (statement.Expression is not null) {
            value = Evaluate(statement.Expression);
        }
        _environment.Define(statement.Token.Lexeme, value);
    }

    public void VisitBlockStatement(BlockStatement statement)
    {
        ExecuteBlock(statement.Statements, new Environment(_environment));
    }

    public void VisitIfStatement(IfStatement statement)
    {
        if (IsTruthy(Evaluate(statement.Condition))) {
            Execute(statement.ThenBranch);
        } else if (statement.ElseBranch != null) {
            Execute(statement.ElseBranch);
        }
    }

    public void VisitWhileStatement(WhileStatement statement)
    {
        while (IsTruthy(Evaluate(statement.Condition))) {
            try
            {
                Execute(statement.Body);
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                continue;
            }
        }
    }

    public void VisitBreakStatement(BreakStatement breakStatement)
    {
        throw new BreakException();
    }

    public void VisitContinueStatement(ContinueStatement continueStatement)
    {
        throw new ContinueException();
    }

    private void ExecuteBlock
    (
        List<Statement> statements,
        Environment environment
    )
    {
        var previous = _environment;
        try
        {
            _environment = environment;
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }
}