using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class SetExpression : Expression
{
    public Expression Object { get; }
    
    public Token Name { get; }
    
    public Expression Value { get; }

    public SetExpression(Expression obj, Token name, Expression value)
    {
        Object = obj;
        Name = name;
        Value = value;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitSetExpression(this);
    }
}