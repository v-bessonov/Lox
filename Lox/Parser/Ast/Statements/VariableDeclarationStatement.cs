using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Statements;

public class VariableDeclarationStatement : Statement
{
    public Expression Expression { get; }

    public Token Token { get; }

    public VariableDeclarationStatement(Token token, Expression expression)
    {
        Token = token;
        Expression = expression;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitVariableDeclarationStatement(this);
    }
}