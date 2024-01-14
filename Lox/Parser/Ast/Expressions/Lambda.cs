using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Statements;
using Lox.Scanner;

namespace Lox.Parser.Ast.Expressions;

public class Lambda : Expression
{
    public Token Name { get; }
    public FunctionDeclarationStatement Function { get; }

    public Lambda(Token name, FunctionDeclarationStatement function)
    {
        Name = name;
        Function = function;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.VisitLambdaExpression(this);
    }
}