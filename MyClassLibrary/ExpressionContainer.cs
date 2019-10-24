using System.Dynamic;
using System.Linq.Expressions;

namespace MyClassLibrary
{
    public sealed class ExpressionContainer : IDynamicMetaObjectProvider, IExpressionContainer
    {
        public Expression Expression { get; set; }

        public bool IsStatic { get; set; }

        public ExpressionContainer(Expression expression)
        {
            this.Expression = expression;
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            // Here we suspect that the value of the parameter argument is not correct.

            return new ExpressionRecorder(parameter, BindingRestrictions.Empty, this);
        }

        public static dynamic Wrap(object instance)
        {
            return new ExpressionContainer(Expression.Constant(instance, instance.GetType()));
        }
    }
}
