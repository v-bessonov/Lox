using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class Call : Expression
{
    public Expression Callee { get; }
    public Token Parenthesis { get; }
    public List<Expression> Arguments { get; }
    

    public Call(Expression callee, Token parenthesis, List<Expression> arguments)
    {
        Callee = callee;
        Parenthesis = parenthesis;
        Arguments = arguments;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitCallExpression(this);
    }
}