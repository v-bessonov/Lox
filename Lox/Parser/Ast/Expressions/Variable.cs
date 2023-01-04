using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class Variable : Expression
{
    public Token Token { get; }

    public Variable(Token token)
    {
        Token = token;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitVariableExpression(this);
    }
}