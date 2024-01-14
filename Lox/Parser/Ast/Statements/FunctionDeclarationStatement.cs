using Lox.Parser.Ast.Interfaces;
using Lox.Scanner;

namespace Lox.Parser.Ast.Statements;

public class FunctionDeclarationStatement : Statement
{
    public Token Name { get; }
    
    public List<Token> Params { get; }
    
    public List<Statement> Body { get; }

    public FunctionDeclarationStatement(Token name, List<Token> @params, List<Statement> body)
    {
        Name = name;
        Params = @params;
        Body = body;
    }

    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.VisitFunctionDeclarationStatement(this);
    }
}