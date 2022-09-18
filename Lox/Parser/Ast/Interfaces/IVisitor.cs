using Lox.Parser.Ast.Expressions;

namespace Lox.Parser.Ast.Interfaces;

public interface IVisitor<T>
{
    T VisitBinaryExpression(Binary expression);
    T VisitGroupingExpression(Grouping expression);
    T VisitLiteralExpression(Literal expression);
    T VisitUnaryExpression(Unary expression);
}