using Lox.Parser.Ast.Functions;
using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Klass;

public class LoxClass : ILoxCallable
{
    public string Name { get; }
    
    public Dictionary<string, LoxFunction> Methods { get; }
    
    public LoxClass(string name, Dictionary<string, LoxFunction> methods) {
        Name = name;
        Methods = methods;
    }
    
    public override String ToString() {
        return Name;
    }

    public int Arity => 0;
    public object? Call(Interpreter.Interpreter interpreter, List<object> arguments)
    {
        var instance = new LoxInstance(this);
        return instance;
    }
    
    public LoxFunction FindMethod(string name) {
        if (Methods.ContainsKey(name)) {
            return Methods[name];
        }
        return null;
    }
}