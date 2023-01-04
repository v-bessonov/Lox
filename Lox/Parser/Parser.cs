using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Statements;
using Lox.Scanner;

namespace Lox.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }
    
    public List<Statement> Parse() {
        List<Statement> statements = new();
        while (!IsAtEnd()) {
            statements.Add(Declaration());
        }
        return statements;
    }
    
    private Statement Declaration() {
        try {
            if (Match(TokenType.VAR))
            {
                return VariableDeclaration();
            }
            return Statement();
        } catch (ParseError error) {
            Synchronize();
            return null;
        }
    }

    private Statement VariableDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");
        Expression initializer = null;
        if (Match(TokenType.EQUAL))
        {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new VariableDeclarationStatement(name, initializer);
    }

    public Expression ParseExpression() {
        try {
            return Expression();
        } catch (ParseError error) {
            return null;
        }
    }

    private Statement Statement()
    {
        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }

        return ExpressionStatement();
    }
    
    private Statement PrintStatement() {
        var expression = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new PrintStatement(expression);
    }
    
    private Statement ExpressionStatement() {
        var expression = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new ExpressionStatement(expression);
    }

    private Expression Expression() {
        return Equality();
    }

    private Expression Equality()
    {
        var expression = Comparison();
        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            var operation = Previous();
            var right = Comparison();
            expression = new Binary(expression, operation, right);
        }

        return expression;
    }

    private Expression Comparison()
    {
        var expression = Term();
        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            var operation = Previous();
            var right = Term();
            expression = new Binary(expression, operation, right);
        }

        return expression;
    }

    private Expression Term() {
        var expression = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS)) {
            var operation = Previous();
            var right = Factor();
            expression = new Binary(expression, operation, right);
        }
        return expression;
    }

    private Expression Factor()
    {
        var expression = Unar();
        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            var operation = Previous();
            var right = Unar();
            expression = new Binary(expression, operation, right);
        }

        return expression;
    }

    private Expression Unar()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            var operation = Previous();
            var right = Unar();
            return new Unary(operation, right);
        }

        return Primary();
    }
    
    private Expression Primary() {
        if (Match(TokenType.FALSE))
        {
            return new Literal(false);
        }

        if (Match(TokenType.TRUE))
        {
            return new Literal(true);
        }

        if (Match(TokenType.NIL))
        {
            return new Literal(null);
        }

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Literal(Previous().Literal);
        }
        
        if (Match(TokenType.IDENTIFIER)) {
            return new Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN)) {
            Expression
                expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
        }
        
        throw Error(Peek(), "Expect expression.");
    }
    
    

    private bool Match(params TokenType[] types) {
        foreach (var type in types)
        {
            if (Check(type)) {
                Advance();
                return true;
            }
        }
        return false;
    }
    
    private Token Consume(TokenType type, String message) {
        if (Check(type))
        {
            return Advance();
        }
        throw Error(Peek(), message);
    }
    
    private ParseError Error(Token token, String message) {
        Lox.Error(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();
        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;
            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    private bool Check(TokenType type) {
        if (IsAtEnd())
        {
            return false;
        }
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }

        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }
}