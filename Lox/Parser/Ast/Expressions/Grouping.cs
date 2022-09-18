using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Expressions;

public class Grouping : Expression
{
    public Expression GroupingExpression { get; }

    public Grouping(Expression expression)
    {
        GroupingExpression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingExpression(this);
    }
}