using Lox.Scanner;

namespace Lox.Interpreter;

public class Environment
{
    private readonly Dictionary<string, object> _values = new();

    public object Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme];
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Define(string name, object value) {
        _values.Add(name, value);
    }
}