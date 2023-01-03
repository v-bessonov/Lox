using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class Unary : Expression
{
    public Token Operation { get; }
    public Expression Right { get; }

    public Unary(Token operation, Expression right)
    {
        Operation = operation;
        Right = right;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitUnaryExpression(this);
    }
}