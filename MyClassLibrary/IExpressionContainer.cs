using System.Linq.Expressions;

namespace MyClassLibrary
{
    public interface IExpressionContainer
    {
        Expression Expression { get; }

        bool IsStatic { get; }
    }
}