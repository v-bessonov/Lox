using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class ThisExpression : Expression
{
    public Token Keyword { get; }

    public ThisExpression(Token keyword)
    {
        Keyword = keyword;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitThisExpression(this);
    }
}