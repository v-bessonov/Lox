using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class SuperExpression : Expression
{
    public Token Keyword { get; }
    
    public Token Method { get; }

    public SuperExpression(Token keyword, Token method)
    {
        Keyword = keyword;
        Method = method;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitSuperExpression(this);
    }
}