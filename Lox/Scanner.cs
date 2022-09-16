using System.Collections.Generic;
using System.Globalization;

namespace Lox;


/// <summary>
/// Scanner
/// </summary>
public class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    public Scanner(string source)
    {
        _source = source;
    }
    
    public List<Token> ScanTokens() {
        while (!IsAtEnd()) {
            // We are at the beginning of the next lexeme.
            _start = _current;
            ScanToken();
        }
        _tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));
        return _tokens;
    }
    
    private bool IsAtEnd() {
        return _current >= _source.Length;
    }
    
    private void ScanToken() {
        var ch = Advance();
        switch (ch)
        {
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }else if (Match('*'))
                {
                    while (!(Peek() == '*' && PeekNext() == '/'))
                    {
                        if (Peek() == '\0')
                        {
                            Lox.Error(_line, "Missing close tag");
                            break;
                        }

                        Advance();
                    }
                    if (!IsAtEnd())
                    {
                        Advance();
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }

                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;
            case '\n':
                _line++;
                break;
            case '"':
                StringScan();
                break;
            default:
                if (IsDigit(ch))
                {
                    Number();
                }
                else if (IsAlpha(ch))
                {
                    Identifier();
                }
                else
                {
                    Lox.Error(_line, "Unexpected character.");
                }

                break;
        }
    }
    
    private bool IsDigit(char ch) {
        return ch >= '0' && ch <= '9';
    }
    
    private char Advance() {
        _current++;
        return _source[_current - 1];
    }
    
    private void AddToken(TokenType type) {
        AddToken(type, null);
    }
    
    private void AddToken(TokenType type, object literal) {
        var text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line));
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_current] != expected)
        {
            return false;
        }
        _current++;
        return true;
    }
    
    private char Peek() {
        if (IsAtEnd())
        {
            return '\0';
        }
        return _source[_current];
    }
    
    private void StringScan() {
        while (Peek() != '"' && !IsAtEnd()) {
            if (Peek() == '\n')
            {
                _line++;
            }
            Advance();
        }
        if (IsAtEnd()) {
            Lox.Error(_line, "Unterminated string.");
            return;
        }
        // The closing ".
        Advance();
        // Trim the surrounding quotes.
        var value = _source.Substring(_start + 1, (_current - 1) - (_start + 1));
        AddToken(TokenType.STRING, value);
    }

    private void Number()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }
        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
           // Consume the "."
            Advance();
            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        double.TryParse(_source.Substring(_start, _current - _start), NumberStyles.Any, CultureInfo.InvariantCulture,
            out var val);
        AddToken(TokenType.NUMBER, val);
    }
    
    private char PeekNext() {
        if (_current + 1 >= _source.Length)
        {
            return '\0';
        }
        return _source[_current + 1];
    }
    private void Identifier() {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }
        var text = _source.Substring(_start, _current - _start);
        Keywords.LoxKeyWords.TryGetValue(text, out var type);
        
        type ??= TokenType.IDENTIFIER;
        AddToken(type.Value);
    }
    
    private bool IsAlpha(char ch)
    {
        return (ch >= 'a' && ch <= 'z') ||
               (ch >= 'A' && ch <= 'Z') ||
               ch == '_';
    }
    
    private bool IsAlphaNumeric(char ch) {
        return IsAlpha(ch) || IsDigit(ch);
    }
}