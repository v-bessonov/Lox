using Lox.Scanner;

namespace Lox.Interpreter;

public class Environment
{
    private Environment _enclosing;
    private readonly Dictionary<string, object> _values = new();

    public Environment() {
        _enclosing = null;
    }
    public Environment(Environment enclosing)
    {
        _enclosing = enclosing;
    }

    public object Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme];
        }

        if (_enclosing != null)
        {
            return _enclosing.Get(name);
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Define(string name, object value) {
        _values.Add(name, value);
    }

    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }
        
        if (_enclosing != null) {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}