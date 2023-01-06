using Lox.Parser.Ast.Statements;

namespace Lox.Parser.Ast.Interfaces;

public interface IStatementVisitor
{
    void VisitPrintStatement(PrintStatement statement);
    void VisitExpressionStatement(ExpressionStatement statement);
    void VisitVariableDeclarationStatement(VariableDeclarationStatement statement);
    void VisitBlockStatement(BlockStatement statement);
}