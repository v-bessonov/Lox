using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public class WhileStatement : Statement
{
    public Expression Condition { get; }

    public Statement Body { get; }

    public WhileStatement(Expression condition, Statement body)
    {
        Condition = condition;
        Body = body;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitWhileStatement(this);
    }
}