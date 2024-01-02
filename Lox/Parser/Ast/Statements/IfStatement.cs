using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public class IfStatement : Statement
{
    public Expression Condition { get; }

    public Statement ThenBranch { get; }
    
    public Statement ElseBranch { get; }

    public IfStatement(Expression condition, Statement thenBranch, Statement elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitIfStatement(this);
    }
}