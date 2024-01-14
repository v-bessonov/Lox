using Lox.Parser.Ast.Expressions;

namespace Lox.Parser.Ast.Interfaces;

public interface IExpressionVisitor<T>
{
    T VisitBinaryExpression(Binary expression);
    T VisitGroupingExpression(Grouping expression);
    T VisitLiteralExpression(Literal expression);
    T VisitUnaryExpression(Unary expression);
    T VisitVariableExpression(Variable expression);
    T VisitAssignmentExpression(Assignment expression);
    T VisitLogicalExpression(Logical expression);
    T VisitCallExpression(Call expression);
    T VisitLambdaExpression(Lambda expression);
}