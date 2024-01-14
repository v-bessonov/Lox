namespace Lox.Parser.Ast.Exceptions;

public class ReturnException : Exception
{
    public object? Value { get; set; }

    public ReturnException(object? value)
    {
        Value = value;
    }
}