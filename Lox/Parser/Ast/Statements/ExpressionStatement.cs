using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public class ExpressionStatement : Statement
{
    public Expression  Expression { get; }

    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitExpressionStatement(this);
    }
}