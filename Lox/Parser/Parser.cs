using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Statements;
using Lox.Scanner;

namespace Lox.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;
    private int _loopLevel = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<Statement> Parse()
    {
        List<Statement> statements = new();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Statement Declaration()
    {
        try
        {
            if (Match(TokenType.CLASS))
            {
                return ClassDeclaration();
            }
            
            if (Match(TokenType.FUN))
            {
                return FunctionDeclaration("function");
            }

            if (Match(TokenType.VAR))
            {
                return VariableDeclaration();
            }

            return Statement();
        }
        catch (ParseError error)
        {
            Synchronize();
            return null;
        }
    }

    private Statement ClassDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expect class name.");
        
        Variable superclass = null;
        if (Match(TokenType.LESS)) {
            Consume(TokenType.IDENTIFIER, "Expect superclass name.");
            superclass = new Variable(Previous());
        }

        Consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");
        
        var methods = new List<FunctionDeclarationStatement>();
        
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
            methods.Add(FunctionDeclaration("method"));
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");
        
        return new ClassDeclarationStatement(name, superclass, methods);
    }

    private FunctionDeclarationStatement FunctionDeclaration(String kind)
    {
        Token name;
        
        if (kind == "lambda") {
            var anon = $"__anon_${Guid.NewGuid()}";
            name = new Token(TokenType.STRING, anon, anon, Previous().Line);
        } else {
            name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
        }

        Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");

        var parameters = new List<Token>();

        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters.");
                }

                parameters.Add(
                    Consume(TokenType.IDENTIFIER, "Expect parameter name."));
            } while (Match(TokenType.COMMA));
        }

        Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

        Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");

        var body = Block();

        return new FunctionDeclarationStatement(name, parameters, body);
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

    public Expression ParseExpression()
    {
        try
        {
            return Expression();
        }
        catch (ParseError error)
        {
            return null;
        }
    }

    private Statement Statement()
    {
        if (Match(TokenType.FOR))
        {
            return ForStatement();
        }

        if (Match(TokenType.IF))
        {
            return IfStatement();
        }

        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }

        if (Match(TokenType.RETURN))
        {
            return ReturnStatement();
        }

        if (Match(TokenType.WHILE))
        {
            return WhileStatement();
        }

        if (Match(TokenType.BREAK))
        {
            return BreakStatement();
        }

        if (Match(TokenType.CONTINUE))
        {
            return ContinueStatement();
        }

        if (Match(TokenType.LEFT_BRACE))
        {
            return new BlockStatement(Block());
        }

        return ExpressionStatement();
    }

    private Statement ForStatement()
    {
        _loopLevel += 1;
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");
        Statement initializer;
        if (Match(TokenType.SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(TokenType.VAR))
        {
            initializer = VariableDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        Expression condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

        Expression increment = null;
        if (!Check(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }

        Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

        var body = Statement();

        if (increment != null)
        {
            body = new BlockStatement(new List<Statement> { body, new ExpressionStatement(increment) });
        }

        if (condition == null)
        {
            condition = new Literal(true);
        }

        body = new WhileStatement(condition, body);

        if (initializer != null)
        {
            body = new BlockStatement(new List<Statement> { initializer, body });
        }

        _loopLevel -= 1;

        return body;
    }

    private Statement WhileStatement()
    {
        _loopLevel += 1;
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
        var body = Statement();
        var whileStatement = new WhileStatement(condition, body);
        _loopLevel -= 1;
        return whileStatement;
    }

    private Statement IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");
        var thenBranch = Statement();
        Statement elseBranch = null;
        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }

    private List<Statement> Block()
    {
        List<Statement> statements = new();
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private Statement BreakStatement()
    {
        if (_loopLevel <= 0)
        {
            throw Error(Previous(), "Expect 'break' inside a loop.");
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new BreakStatement();
    }

    private Statement ContinueStatement()
    {
        if (_loopLevel <= 0)
        {
            throw Error(Previous(), "Expect 'continue' inside a loop.");
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new ContinueStatement();
    }

    private Statement PrintStatement()
    {
        var expression = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new PrintStatement(expression);
    }

    private Statement ReturnStatement()
    {
        var keyword = Previous();
        Expression? value = null;
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
        return new ReturnStatement(keyword, value);
    }

    private Statement ExpressionStatement()
    {
        var expression = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new ExpressionStatement(expression);
    }

    private Expression Expression()
    {
        return Assignment();
    }

    private Expression Assignment()
    {
        var expression = Or();
        if (Match(TokenType.EQUAL))
        {
            var equals = Previous();
            var value = Assignment();
            if (expression is Variable)
            {
                var token = ((Variable)expression).Token;
                return new Assignment(token, value);
            }else if (expression is GetExpression) {
                var get = (GetExpression)expression;
                return new SetExpression(get.Object, get.Name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expression;
    }

    private Expression Or()
    {
        var expression = And();
        while (Match(TokenType.OR))
        {
            var operation = Previous();
            Expression right = And();
            expression = new Logical(expression, operation, right);
        }

        return expression;
    }

    private Expression And()
    {
        var expression = Equality();
        while (Match(TokenType.AND))
        {
            var operation = Previous();
            var right = Equality();
            expression = new Logical(expression, operation, right);
        }

        return expression;
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

    private Expression Term()
    {
        var expression = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            var operation = Previous();
            var right = Factor();
            expression = new Binary(expression, operation, right);
        }

        return expression;
    }

    private Expression Factor()
    {
        var expression = UnaryExpression();
        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            var operation = Previous();
            var right = UnaryExpression();
            expression = new Binary(expression, operation, right);
        }

        return expression;
    }

    private Expression UnaryExpression()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            var operation = Previous();
            var right = UnaryExpression();
            return new Unary(operation, right);
        }

        return CallExpression();
    }

    private Expression CallExpression()
    {
        var expression = Primary();
        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expression = FinishCall(expression);
            }
            else if (Match(TokenType.DOT)) {
                var name = Consume(TokenType.IDENTIFIER,
                    "Expect property name after '.'.");
                expression = new GetExpression(expression, name);
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private Expression FinishCall(Expression callee)
    {
        var arguments = new List<Expression>();
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }

                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }

        var paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");
        return new Call(callee, paren, arguments);
    }

    private Expression Primary()
    {
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
        
        if (Match(TokenType.SUPER)) {
            var keyword = Previous();
            Consume(TokenType.DOT, "Expect '.' after 'super'.");
            var method = Consume(TokenType.IDENTIFIER, "Expect superclass method name.");
            return new SuperExpression(keyword, method);
        }
        
        if (Match(TokenType.THIS))
            return new ThisExpression(Previous());

        if (Match(TokenType.IDENTIFIER))
        {
            return new Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            Expression
                expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
        }

        if (Match(TokenType.FUN) && Check(TokenType.LEFT_PAREN))
        {
            var functionDeclaration = FunctionDeclaration("lambda");
            return new Lambda(functionDeclaration.Name, functionDeclaration);
        }

        throw Error(Peek(), "Expect expression.");
    }


    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Consume(TokenType type, String message)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, String message)
    {
        LoxLang.Error(token, message);
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
                case TokenType.BREAK:
                case TokenType.CONTINUE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    private bool Check(TokenType type)
    {
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
        var ff = _tokens[_current];
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }
}