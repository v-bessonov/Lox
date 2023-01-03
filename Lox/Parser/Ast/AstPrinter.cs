using System.Globalization;
using System.Text;
using Lox.Parser.Ast.Expressions;
using Lox.Parser.Ast.Interfaces;

namespace Lox.Parser.Ast;

public class AstPrinter: IExpressionVisitor<string>
{
    public string Print(Expression expression) {
        return expression.Accept(this);
    }

    public string VisitBinaryExpression(Binary expression)
    {
        return Parenthesize(expression.Operation.Lexeme, expression.Left, expression.Right);
    }

    public string VisitGroupingExpression(Grouping expression)
    {
        return Parenthesize("group", expression.GroupingExpression);
    }

    public string VisitLiteralExpression(Literal expression)
    {
        return expression.Value is null ? "nil" : Convert.ToString(expression.Value, CultureInfo.InvariantCulture);
    }

    public string VisitUnaryExpression(Unary expression)
    {
        return Parenthesize(expression.Operation.Lexeme, expression.Right);
    }

    private string Parenthesize(string name, params Expression[] expressions)
    {
        var builder = new StringBuilder();
        builder.Append("(").Append(name);
        foreach (var expression in expressions)
        {
            builder.Append(" ");
            builder.Append(expression.Accept(this));
        }

        builder.Append(")");
        return builder.ToString();
    }
}