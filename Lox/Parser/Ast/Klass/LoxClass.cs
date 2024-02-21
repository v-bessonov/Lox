using Lox.Parser.Ast.Functions;
using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Klass;

public class LoxClass : ILoxCallable
{
    public string Name { get; }
    
    public Dictionary<string, LoxFunction> Methods { get; }
    
    public LoxClass SuperClass { get; }
    
    public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods) {
        Name = name;
        SuperClass = superclass;
        Methods = methods;
    }
    
    public override String ToString() {
        return Name;
    }

    public int Arity()
    {
        var initializer = FindMethod("init");
        if (initializer is null) return 0;
        return initializer.Arity();
    }
    public object? Call(Interpreter.Interpreter interpreter, List<object> arguments)
    {
        var instance = new LoxInstance(this);
        var initializer = FindMethod("init");
        if (initializer is not null) {
            initializer.Bind(instance).Call(interpreter, arguments);
        }
        return instance;
    }
    
    public LoxFunction FindMethod(string name) {
        if (Methods.ContainsKey(name)) {
            return Methods[name];
        }
        
        if (SuperClass is not null) {
            return SuperClass.FindMethod(name);
        }
        
        return null;
    }
}