using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class Logical : Expression
{
    public Expression Left { get; }
    public Token Operation { get; }
    public Expression Right { get; }

    public Logical(Expression left, Token operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitLogicalExpression(this);
    }
}