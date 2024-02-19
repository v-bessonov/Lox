namespace Lox.Parser.Ast.Interfaces;

public interface ILoxCallable
{
    int Arity();
    object? Call(Interpreter.Interpreter interpreter, List<object> arguments);
}