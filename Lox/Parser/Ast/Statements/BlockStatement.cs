using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public class BlockStatement : Statement
{
    public List<Statement> Statements { get; }

    public BlockStatement(List<Statement> statements)
    {
        Statements = statements;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitBlockStatement(this);
    }
}