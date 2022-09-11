using System.Collections.Generic;

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
            default:
                Lox.Error(_line, "Unexpected character.");
                break;
        }
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
        if (_source[_current] != expected) return false;
        _current++;
        return true;
    }
}