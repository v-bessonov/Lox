using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class GetExpression : Expression
{
    public Expression Object { get; }
    
    public Token Name { get; }

    public GetExpression(Expression obj, Token name)
    {
        Object = obj;
        Name = name;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitGetExpression(this);
    }
}