using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public class BreakStatement : Statement
{
    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitBreakStatement(this);
    }
}