﻿using Lox.Parser.Ast.Interfaces;

namespace Lox.Interpreter.Native;

public class Clock : ILoxCallable
{
    public int Arity() => 0;
    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return Convert.ToDouble(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds());
    }

    public override string ToString()
    {
        return "<native fn clock>";
    }
}