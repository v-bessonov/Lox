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
        // if (_enclosing._values.ContainsKey("this"))
        // {
        //     _values.Add("this", enclosing._values["this"]);
        // }
    }

    public void Copy()
    {
        foreach (var item in _enclosing._values)
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

        if (_enclosing != null)
        {
            return _enclosing.Get(name);
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
            environment = environment._enclosing;
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
        while (environment._enclosing is not null)
        {
            environment = environment._enclosing;
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
        
        if (_enclosing != null) {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
    
    public void AssignAt(int distance, Token name, object value) {
        Ancestor(distance)._values.Add(name.Lexeme, value);
    }
}