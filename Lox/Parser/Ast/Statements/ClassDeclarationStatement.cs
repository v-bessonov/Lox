using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Statements;

public class ClassDeclarationStatement : Statement
{
    public List<FunctionDeclarationStatement> Methods { get; }

    public Token Name { get; }

    public ClassDeclarationStatement(Token name, List<FunctionDeclarationStatement> methods)
    {
        Name = name;
        Methods = methods;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitClassDeclarationStatement(this);
    }
}