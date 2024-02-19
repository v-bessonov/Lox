using Lox.Parser.Ast.Exceptions;
using Lox.Parser.Ast.Interfaces;
using Lox.Parser.Ast.Klass;
using Lox.Parser.Ast.Statements;
using Environment = Lox.Interpreter.Environment;

namespace Lox.Parser.Ast.Functions;

public class LoxFunction : ILoxCallable
{
    public FunctionDeclarationStatement Declaration { get; }
    private Environment _closure { get; }
    
    private readonly bool _isInitializer;

    public LoxFunction(FunctionDeclarationStatement declaration, Environment closure, bool isInitializer)
    {
        _closure = closure;
        _isInitializer = isInitializer;
        Declaration = declaration;
    }

    public int Arity() => Declaration.Params.Count;

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
            if (_isInitializer) return _closure.GetAt(0, "this");
            return returnException.Value;
        }

        if (_isInitializer) return _closure.GetAt(0, "this");
        return null;
    }
    
    public LoxFunction Bind(LoxInstance instance) {
        var environment = new Environment(_closure);
        environment.Define("this", instance);
        return new LoxFunction(Declaration, environment, _isInitializer);
    }

    public override string ToString()
    {
        return $"<fn {Declaration.Name.Lexeme}";
    }
}