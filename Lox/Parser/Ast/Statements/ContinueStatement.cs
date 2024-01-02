using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public class ContinueStatement : Statement
{
    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitContinueStatement(this);
    }
}