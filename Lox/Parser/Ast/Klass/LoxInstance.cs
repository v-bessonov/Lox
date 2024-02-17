using Lox.Interpreter;
using Lox.Scanner;

namespace Lox.Parser.Ast.Klass;

public class LoxInstance
{
    private LoxClass _klass;
    private readonly Dictionary<string, object> _fields = new();
    
    public LoxInstance(LoxClass klass) {
        _klass = klass;
    }
    
    public object Get(Token name) {
        if (_fields.ContainsKey(name.Lexeme)) {
            return _fields[name.Lexeme];
        }
        
        var method = _klass.FindMethod(name.Lexeme);
        if (method is not null) return method.Bind(this);
        
        throw new RuntimeError(name,
            "Undefined property '" + name.Lexeme + "'.");
    }
    
    public void Set(Token name, Object value) {
        _fields.Add(name.Lexeme, value);
    }
    
    
    
    public override string ToString() {
        return $"{_klass.Name} instance";
    }
}