using Lox.Parser.Ast.Statements;

namespace Lox.Parser.Ast.Interfaces;

public interface IStatementVisitor
{
    void VisitPrintStatement(PrintStatement statement);
    void VisitExpressionStatement(ExpressionStatement statement);
    void VisitVariableDeclarationStatement(VariableDeclarationStatement statement);
    void VisitBlockStatement(BlockStatement statement);
    void VisitIfStatement(IfStatement statement);
    void VisitWhileStatement(WhileStatement statement);
    void VisitBreakStatement(BreakStatement statement);
    void VisitContinueStatement(ContinueStatement statement);
    void VisitFunctionDeclarationStatement(FunctionDeclarationStatement statement);
    void VisitReturnStatement(ReturnStatement statement);
    void VisitClassDeclarationStatement(ClassDeclarationStatement statement);
}