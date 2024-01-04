using Lox.Parser.Ast.Interfaces;

namespace Lox.Interpreter.Native;

public class Clock : ILoxCallable
{
    public int Arity { get; set; } = 0;
    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    public override string ToString()
    {
        return "<native fn clock>";
    }
}