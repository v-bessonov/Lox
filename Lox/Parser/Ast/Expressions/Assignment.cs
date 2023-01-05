using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class Assignment : Expression
{
    public Token Token { get; }
    public Expression Expression { get; }

    public Assignment(Token token, Expression expression)
    {
        Token = token;
        Expression = expression;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitAssignmentExpression(this);
    }
}