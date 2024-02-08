using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Statements;
using Lox.Scanner;

namespace Lox.Interpreter;

public class Resolver : IExpressionVisitor<object>, IStatementVisitor
{
    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> _scopes = new();
    private FunctionType _currentFunction = FunctionType.NONE;

    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    public void Resolve(List<Statement> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }

    private void Resolve(Statement statement)
    {
        statement.Accept(this);
    }

    private void Resolve(Expression expression)
    {
        expression.Accept(this);
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    public object VisitBinaryExpression(Binary expression)
    {
        Resolve(expression.Left);
        Resolve(expression.Right);
        return null;
    }

    public object VisitGroupingExpression(Grouping expression)
    {
        Resolve(expression.GroupingExpression);
        return null;
    }

    public object VisitLiteralExpression(Literal expression)
    {
        return null;
    }

    public object VisitUnaryExpression(Unary expression)
    {
        Resolve(expression.Right);
        return null;
    }

    public object VisitVariableExpression(Variable expression)
    {
        if (_scopes.Any())
        {
            var scope = _scopes.Peek();
            if (scope.TryGetValue(expression.Token.Lexeme, out var val) && !val)
            { 
                LoxLang.Error(expression.Token, "Can't read local variable in its own initializer.");
            }
        }

        ResolveLocal(expression, expression.Token);
        return null;
    }

    private void ResolveLocal(Expression expression, Token name)
    {
        for (var i = _scopes.Count - 1; i >= 0; i--)
        {
            if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
            {
                _interpreter.Resolve(expression, _scopes.Count - 1 - i);
                return;
            }
        }
    }

    public object VisitAssignmentExpression(Assignment expression)
    {
        Resolve(expression.Expression);
        ResolveLocal(expression, expression.Token);
        return null;
    }

    public object VisitLogicalExpression(Logical expression)
    {
        Resolve(expression.Left);
        Resolve(expression.Right);
        return null;
    }

    public object VisitCallExpression(Call expression)
    {
        Resolve(expression.Callee);
        foreach (var argument in expression.Arguments) {
            Resolve(argument);
        }
        return null;
    }

    public object VisitLambdaExpression(Lambda expression)
    {
        Declare(expression.Function.Name);
        Define(expression.Function.Name);
        ResolveFunction(expression.Function, FunctionType.FUNCTION);
        return null;
    }

    public void VisitPrintStatement(PrintStatement statement)
    {
        Resolve(statement.Expression);
    }

    public void VisitExpressionStatement(ExpressionStatement statement)
    {
        Resolve(statement.Expression);
    }

    public void VisitVariableDeclarationStatement(VariableDeclarationStatement statement)
    {
        Declare(statement.Token);
        if (statement.Expression is not null)
        {
            Resolve(statement.Expression);
        }

        Define(statement.Token);
    }

    private void Declare(Token name)
    {
        if (!_scopes.Any())
            return;

        var scope = _scopes.Peek();
        
        if (scope.ContainsKey(name.Lexeme)) {
            LoxLang.Error(name, "Already variable with this name in this scope.");
        }
        
        scope.Add(name.Lexeme, false);
    }

    private void Define(Token name)
    {
        if (!_scopes.Any())
            return;

        var scope = _scopes.Peek();
        scope[name.Lexeme] = true;
    }

    public void VisitBlockStatement(BlockStatement statement)
    {
        BeginScope();
        Resolve(statement.Statements);
        EndScope();
    }

    public void VisitIfStatement(IfStatement statement)
    {
        Resolve(statement.Condition);
        Resolve(statement.ThenBranch);
        if (statement.ElseBranch != null)
        {
            Resolve(statement.ElseBranch);
        }
    }

    public void VisitWhileStatement(WhileStatement statement)
    {
        Resolve(statement.Condition);
        Resolve(statement.Body);
    }

    public void VisitBreakStatement(BreakStatement statement)
    {
    }

    public void VisitContinueStatement(ContinueStatement statement)
    {
    }

    public void VisitFunctionDeclarationStatement(FunctionDeclarationStatement statement)
    {
        Declare(statement.Name);
        Define(statement.Name);
        ResolveFunction(statement, FunctionType.FUNCTION);
    }

    private void ResolveFunction(FunctionDeclarationStatement function, FunctionType type)
    {
        var enclosingFunction = _currentFunction;
        _currentFunction = type;
        
        BeginScope();

        foreach (var param in function.Params)
        {
            Declare(param);
            Define(param);
        }

        Resolve(function.Body);
        EndScope();
        _currentFunction = enclosingFunction;
    }

    public void VisitReturnStatement(ReturnStatement statement)
    {
        if (_currentFunction == FunctionType.NONE) {
            LoxLang.Error(statement.Keyword, "Can't return from top-level code.");
        }
        if (statement.Value != null) {
            Resolve(statement.Value);
        }
    }
}