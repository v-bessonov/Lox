﻿using Lox.Parser.Ast;
using Lox.Parser.Ast.Expressions;
using Lox.Scanner;
using Expression = System.Linq.Expressions.Expression;

namespace Lox;

/// <summary>
/// Lox compiler
/// </summary>
public class Lox
{
    private static bool _hadError = false;
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

        TestAstPrinter();
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
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
        
    public static void Error(int line, string message) {
        Report(line, "", message);
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
}