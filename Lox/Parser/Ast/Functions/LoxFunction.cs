using Lox.Parser.Ast.Exceptions;
using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Statements;

namespace Lox.Parser.Ast.Functions;

public class LoxFunction : ILoxCallable
{
    public FunctionDeclarationStatement Declaration { get; }
    private Interpreter.Environment _closure { get; }

    public LoxFunction(FunctionDeclarationStatement declaration, Interpreter.Environment closure)
    {
        _closure = closure;
        Declaration = declaration;
    }

    public int Arity => Declaration.Params.Count;

    public object? Call(Interpreter.Interpreter interpreter, List<object> arguments)
    {
        var environment = new Interpreter.Environment(_closure);
        for (var i = 0; i < Declaration.Params.Count; i++)
        {
            environment.Define(Declaration.Params[i].Lexeme, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(Declaration.Body, environment);
        }
        catch (ReturnException returnException)
        {
            return returnException.Value;
        }

        return null;
    }

    public override string ToString()
    {
        return $"<fn {Declaration.Name.Lexeme}";
    }
}