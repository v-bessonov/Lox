using Lox.Interpreter;
using Lox.Parser.Ast;
using Lox.Parser.Ast.Expressions;
using Lox.Scanner;
using Environment = System.Environment;

namespace Lox;

/// <summary>
/// Lox compiler
/// </summary>
public class LoxLang
{
    private static bool _hadError = false;
    private static bool _hadRuntimeError = false;
    private static Interpreter.Interpreter _interpreter = new Interpreter.Interpreter();
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: lox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }

        //TestAstPrinter();
    }

    private static void RunPrompt()
    {
        for (;;)
        {
            Console.WriteLine("> ");
            var line = Console.ReadLine();
            if (line is null)
            {
                break;
            }

            Run(line);
            _hadError = false;
        }
    }


    private static void RunFile(string path)
    {
        var fullPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\{path}";
        var content = File.ReadAllText(fullPath);
        Run(content);
        if (_hadError)
        {
            Environment.Exit(65);
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner.Scanner(source);
        var tokens = scanner.ScanTokens();

        // For now, just print the tokens.
        // foreach (var token in tokens)
        // {
        //     Console.WriteLine(token);
        // }
        //
        // return;

        var parser = new Parser.Parser(tokens);
        //var expression = parser.Parse();
        var statements = parser.Parse();

        //new AstPrinter().Print(statements);

        //return;
        // Stop if there was a syntax error.
        if (_hadError)
        {
            Environment.Exit(65);
        }

        if (_hadRuntimeError)
        {
            Environment.Exit(70);
        }

        //Console.WriteLine(new AstPrinter().Print(expression));
        
        
        var resolver = new Resolver(_interpreter);
        resolver.Resolve(statements);
        
        if (_hadError) return;
        
        //_interpreter.Interpret(expression);
        _interpreter.Interpret(statements);
    }

    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    public static void Error(Token token, String message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    private static void Report
    (
        int line,
        string where,
        string message
    )
    {
        Console.WriteLine($"[line {line}] Error{where}: {message}");
        _hadError = true;
    }

    private static void TestAstPrinter()
    {
        var
            expression = new Binary(
                new Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Literal(123)),
                new Token(TokenType.STAR, "*", null, 1),
                new Grouping(
                    new Literal(45.67)));
        Console.WriteLine(new AstPrinter().Print(expression));
    }
    
    public static void RuntimeError(RuntimeError error) {
        Console.WriteLine($"{error.Message}{Environment.NewLine}[line {error.Token.Line}]");
        _hadRuntimeError = true;
    }
}