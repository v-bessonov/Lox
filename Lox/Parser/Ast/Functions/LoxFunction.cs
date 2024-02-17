using Lox.Parser.Ast.Exceptions;
using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Klass;
using Lox.Parser.Ast.Statements;
using Environment = Lox.Interpreter.Environment;

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
        var environment = new Environment(_closure);
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
    
    public LoxFunction Bind(LoxInstance instance) {
        var environment = new Environment(_closure);
        environment.Define("this", instance);
        return new LoxFunction(Declaration, environment);
    }

    public override string ToString()
    {
        return $"<fn {Declaration.Name.Lexeme}";
    }
}