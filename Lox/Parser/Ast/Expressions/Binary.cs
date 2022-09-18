using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class Binary : Expression
{
    public Expression Left { get; }
    public Token Operation { get; }
    public Expression Right { get; }

    public Binary(Expression left, Token operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpression(this);
    }
}