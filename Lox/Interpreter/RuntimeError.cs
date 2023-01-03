using Lox.Scanner;

namespace Lox.Interpreter;

public class RuntimeError : SystemException
{
    public Token Token { get; }

    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }
}