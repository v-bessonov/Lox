using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Expressions;

public abstract class Expression
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}