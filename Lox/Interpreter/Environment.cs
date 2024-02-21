using Lox.Scanner;

namespace Lox.Interpreter;

public class Environment
{
    public Environment Enclosing { get; }
    private readonly Dictionary<string, object> _values = new();

    public Environment() {
        Enclosing = null;
    }
    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }

    public void Copy()
    {
        foreach (var item in Enclosing._values)
        {
            _values.Add(item.Key, item.Value);
        }
    }

    public object Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme];
        }

        if (Enclosing != null)
        {
            return Enclosing.Get(name);
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Define(string name, object value) {
        _values.Add(name, value);
    }
    
    public object GetAt(int distance, string name)
    {
        return Ancestor(distance)._values[name];
    }
    
    Environment Ancestor(int distance) {
        var environment = this;
        for (var i = 0; i < distance; i++) {
            environment = environment.Enclosing;
        }
        return environment;
    }

    Environment Descendant(int distance)
    {
        var environment = this;
        var allEnvironmentsChain = new List<Environment>
        {
            environment
        };
        while (environment.Enclosing is not null)
        {
            environment = environment.Enclosing;
            allEnvironmentsChain.Add(environment);
        }
        
        allEnvironmentsChain.Reverse();

        return allEnvironmentsChain[distance + 1];
    }

    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }
        
        if (Enclosing != null) {
            Enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
    
    public void AssignAt(int distance, Token name, object value) {
        Ancestor(distance)._values.Add(name.Lexeme, value);
    }
}