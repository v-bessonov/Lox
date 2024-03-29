﻿using Lox.Interpreter.Native;
using Lox.Parser.Ast.Exceptions;
using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Functions;
using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Klass;
using Lox.Parser.Ast.Statements;
using Lox.Scanner;

namespace Lox.Interpreter;

public class Interpreter : IExpressionVisitor<object>, IStatementVisitor
{
    public static readonly Environment Globals = new();

    private Environment _environment = Globals;

    private Dictionary<Expression, int> _locals = new();

    public Interpreter()
    {
        Globals.Define("clock", new Clock());
    }

    public void Interpret(List<Statement> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            LoxLang.RuntimeError(error);
        }
    }

    public void Resolve(Expression expr, int depth)
    {
        _locals.Add(expr, depth);
    }

    public void Interpret(Expression expression)
    {
        try
        {
            var value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        }
        catch (RuntimeError error)
        {
            LoxLang.RuntimeError(error);
        }
    }

    private void Execute(Statement statement)
    {
        statement.Accept(this);
    }

    private string Stringify(object obj)
    {
        if (obj is null)
        {
            return "nil";
        }

        if (obj is double)
        {
            var text = obj.ToString();
            if (text.EndsWith(".0"))
            {
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
        return LookUpVariable(expression.Token, expression);
    }

    private object LookUpVariable(Token name, Expression expression)

    {
        if (_locals.TryGetValue(expression, out var distance))
        {
            return _environment.GetAt(distance, name.Lexeme);
        }

        return Globals.Get(name);
    }

    public object VisitAssignmentExpression(Assignment expression)
    {
        var value = Evaluate(expression.Expression);
        if (_locals.TryGetValue(expression, out var distance))
        {
            _environment.AssignAt(distance, expression.Token, value);
        }
        else
        {
            Globals.Assign(expression.Token, value);
        }

        return value;
    }

    public object VisitLogicalExpression(Logical expression)
    {
        var left = Evaluate(expression.Left);
        if (expression.Operation.Type == TokenType.OR)
        {
            if (IsTruthy(left))
            {
                return left;
            }
        }
        else
        {
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

        if (!(callee is ILoxCallable function))
        {
            throw new RuntimeError(expression.Parenthesis, "Can only call functions and classes.");
        }

        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expression.Parenthesis,
                $"Expected {function.Arity} arguments but got {arguments.Count}.");
        }

        return function.Call(this, arguments);
    }

    public object VisitLambdaExpression(Lambda expression)
    {
        Execute(expression.Function);
        return _environment.Get(expression.Name);
    }

    public object VisitGetExpression(GetExpression expression)
    {
        var obj = Evaluate(expression.Object);
        if (obj is LoxInstance) {
            return ((LoxInstance) obj).Get(expression.Name);
        }
        throw new RuntimeError(expression.Name,
            "Only instances have properties.");
    }

    public object VisitSetExpression(SetExpression expression)
    {
        var obj = Evaluate(expression.Object);
        if (!(obj is LoxInstance)) {
            throw new RuntimeError(expression.Name, "Only instances have fields.");
        }
        var value = Evaluate(expression.Value);
        ((LoxInstance)obj).Set(expression.Name, value);
        return value;
    }

    public object VisitThisExpression(ThisExpression expression)
    {
        return LookUpVariable(expression.Keyword, expression);
    }

    public object VisitSuperExpression(SuperExpression expression)
    {
        var distance = _locals[expression];
        var superclass = (LoxClass)_environment.GetAt(distance, "super");
        
        var obj = (LoxInstance)_environment.GetAt(distance - 1, "this");
        
        var method = superclass.FindMethod(expression.Method.Lexeme);
        
        if (method == null) {
            throw new RuntimeError(expression.Method,
                "Undefined property '" + expression.Method.Lexeme + "'.");
        }
        
        return method.Bind(obj);
    }

    private bool IsTruthy(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is bool)
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

    private void CheckNumberOperand(Token operation, object operand)
    {
        if (operand is double)
        {
            return;
        }

        throw new RuntimeError(operation, "Operand must be a number.");
    }

    private void CheckNumberOperand(Token operation, object left, object right)
    {
        if (left is double && right is double)
        {
            return;
        }

        throw new RuntimeError(operation, "Operands must be a numbers.");
    }

    public object VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.GroupingExpression);
    }

    private object Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    private bool IsEqual(object a, object b)
    {
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
        if (statement.Expression is not null)
        {
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
        if (IsTruthy(Evaluate(statement.Condition)))
        {
            Execute(statement.ThenBranch);
        }
        else if (statement.ElseBranch != null)
        {
            Execute(statement.ElseBranch);
        }
    }

    public void VisitWhileStatement(WhileStatement statement)
    {
        while (IsTruthy(Evaluate(statement.Condition)))
        {
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

    public void VisitFunctionDeclarationStatement(FunctionDeclarationStatement statement)
    {
        var function = new LoxFunction(statement, _environment, false);
        _environment.Define(statement.Name.Lexeme, function);
    }

    public void VisitReturnStatement(ReturnStatement statement)
    {
        object? value = null;
        if (statement.Value is not null) value = Evaluate(statement.Value);
        throw new ReturnException(value);
    }

    public void VisitClassDeclarationStatement(ClassDeclarationStatement statement)
    {
        object? superclass = null;
        if (statement.SuperClass is not null) {
            superclass = Evaluate(statement.SuperClass);
            if (!(superclass is LoxClass)) {
                throw new RuntimeError(statement.SuperClass.Token, "Superclass must be a class.");
            }
        }
        
        _environment.Define(statement.Name.Lexeme, null);
        
        if (statement.SuperClass is not null) {
            _environment = new Environment(_environment);
            _environment.Define("super", superclass);
        }
        
        Dictionary<string, LoxFunction> methods = new();
        foreach (var method in statement.Methods) {
            var function = new LoxFunction(method, _environment, method.Name.Lexeme.Equals("init"));
            methods.Add(method.Name.Lexeme, function);
        }
        var klass = new LoxClass(statement.Name.Lexeme, (LoxClass)superclass, methods);
        
        if (superclass is not null) {
            _environment = _environment.Enclosing;
        }
        
        _environment.Assign(statement.Name, klass);
    }

    public void ExecuteBlock
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