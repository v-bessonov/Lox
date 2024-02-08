using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast.Expressions;

public abstract class Expression
{
    private readonly Guid _id = Guid.NewGuid(); 
    public abstract T Accept<T>(IExpressionVisitor<T> expressionVisitor);
    
    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Expression other)
        {
            return _id == other._id;
        }
        return false;
    }
    
}