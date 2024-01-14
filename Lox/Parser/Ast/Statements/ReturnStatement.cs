using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Statements;

public class ReturnStatement : Statement
{
    public Token Keyword { get; }
    public Expression? Value { get; }

    public ReturnStatement(Token keyword, Expression? value)
    {
        Keyword = keyword;
        Value = value;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitReturnStatement(this);
    }
}