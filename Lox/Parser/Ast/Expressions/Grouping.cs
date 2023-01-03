using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Expressions;

public class Grouping : Expression
{
    public Expression GroupingExpression { get; }

    public Grouping(Expression expression)
    {
        GroupingExpression = expression;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitGroupingExpression(this);
    }
}