using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Expressions;

public class Literal : Expression
{
    public object Value { get; }

    public Literal(object value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}