using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Statements;

public abstract class Statement
{
    public abstract void Accept(IStatementVisitor statementVisitor);
}