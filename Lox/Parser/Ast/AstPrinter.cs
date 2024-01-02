﻿using System.Globalization;
using System.Text;
using Lox.Interpreter;
using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Statements;

namespace Lox.Parser.Ast;

public class AstPrinter: IExpressionVisitor<string>, IStatementVisitor
{
    
    public void Print(List<Statement> statements)
    {
        try {
            foreach (var statement in statements)
            {
                statement.Accept(this);
                //Console.WriteLine($"{new string(' ', indent * 2)}{root.Value}");
            }
        } catch (RuntimeError error) {
            Lox.RuntimeError(error);
        }
    }
    
    public string Print(Expression expression) {
        return expression.Accept(this);
    }

    public string VisitBinaryExpression(Binary expression)
    {
        return Parenthesize(expression.Operation.Lexeme, expression.Left, expression.Right);
    }

    public string VisitGroupingExpression(Grouping expression)
    {
        return Parenthesize("group", expression.GroupingExpression);
    }

    public string VisitLiteralExpression(Literal expression)
    {
        return expression.Value is null ? "nil" : Convert.ToString(expression.Value, CultureInfo.InvariantCulture);
    }

    public string VisitUnaryExpression(Unary expression)
    {
        return Parenthesize(expression.Operation.Lexeme, expression.Right);
    }

    public string VisitVariableExpression(Variable expression)
    {
        return $"var {expression.Token.Lexeme}";
    }

    public string VisitAssignmentExpression(Assignment expression)
    {
        var expressionAssigned = expression.Expression.Accept(this);
        return $"(Expression; {nameof(Assignment)}; Token {expression.Token}  = ; Expression assigned: {expressionAssigned})";
    }

    public string VisitLogicalExpression(Logical expression)
    {
        return Parenthesize(expression.Operation.Lexeme, expression.Left, expression.Right);
    }

    private string Parenthesize(string name, params Expression[] expressions)
    {
        var builder = new StringBuilder();
        builder.Append("(").Append(name);
        foreach (var expression in expressions)
        {
            builder.Append(" ");
            builder.Append(expression.Accept(this));
        }

        builder.Append(")");
        return builder.ToString();
    }

    public void VisitPrintStatement(PrintStatement statement)
    {
        var expression = statement.Expression.Accept(this);
        Console.WriteLine($"Statement: {nameof(PrintStatement)}; Expression: {expression}");
    }

    public void VisitExpressionStatement(ExpressionStatement statement)
    {
        var expression = statement.Expression?.Accept(this) ?? "No value";
        Console.WriteLine($"Statement: {nameof(ExpressionStatement)}; ;Expression: {expression}");
    }

    public void VisitVariableDeclarationStatement(VariableDeclarationStatement statement)
    {
        var expression = statement.Expression?.Accept(this) ?? "No value";
        Console.WriteLine($"Statement: {nameof(VariableDeclarationStatement)}; Token: {statement.Token} ;Expression: {expression}");
    }

    public void VisitBlockStatement(BlockStatement statement)
    {
        Console.WriteLine(nameof(BlockStatement));
        foreach (var st in statement.Statements)
        {
            st.Accept(this);
        }
    }

    public void VisitIfStatement(IfStatement statement)
    {
        Console.WriteLine(nameof(IfStatement));
    }

    public void VisitWhileStatement(WhileStatement statement)
    {
        var condition = statement.Condition?.Accept(this) ?? "No value";
        Console.WriteLine($"Statement: {nameof(WhileStatement)}; Condition: {condition}");
        statement.Body.Accept(this);
    }

    public void VisitBreakStatement(BreakStatement breakStatement)
    {
        Console.WriteLine($"Statement: {nameof(BreakStatement)};");
    }

    public void VisitContinueStatement(ContinueStatement continueStatement)
    {
        Console.WriteLine($"Statement: {nameof(ContinueStatement)};");
    }
}