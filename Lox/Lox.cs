using System;
using System.Collections.Generic;
using System.IO;

namespace Lox
{
    /// <summary>
    /// Lox compiler
    /// </summary>
    public class Lox
    {
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
            }
        }


        private static void RunFile(string path)
        {
            var content = File.ReadAllText(path);
            Run(content);
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();

            // For now, just print the tokens.
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
}