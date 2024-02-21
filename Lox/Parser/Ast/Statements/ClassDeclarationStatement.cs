using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Statements;

public class ClassDeclarationStatement : Statement
{
    public List<FunctionDeclarationStatement> Methods { get; }

    public Token Name { get; }
    
    public Variable SuperClass { get; }

    public ClassDeclarationStatement(Token name, Variable superClass, List<FunctionDeclarationStatement> methods)
    {
        Name = name;
        SuperClass = superClass;
        Methods = methods;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitClassDeclarationStatement(this);
    }
}