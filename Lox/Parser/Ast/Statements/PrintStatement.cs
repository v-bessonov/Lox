using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public class PrintStatement : Statement
{
    public Expression  Expression { get; }

    public PrintStatement(Expression expression)
    {
        Expression = expression;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitPrintStatement(this);
    }
}