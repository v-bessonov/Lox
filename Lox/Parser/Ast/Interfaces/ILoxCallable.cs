﻿namespace Lox.Parser.Ast.Interfaces;

public interface ILoxCallable
{
    int Arity { get; set; }
    object Call(Interpreter.Interpreter interpreter, List<object> arguments);
}