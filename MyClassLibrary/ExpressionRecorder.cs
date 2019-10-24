using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyClassLibrary
{
    internal sealed class ExpressionRecorder : DynamicMetaObject
    {
        public ExpressionRecorder(Expression expression, BindingRestrictions restrictions)
            : base(expression, restrictions)
        { }

        public ExpressionRecorder(Expression expression, BindingRestrictions restrictions, object value)
            : base(expression, restrictions, value)
        { }

        private static DynamicMetaObject CreateRecorder(Expression expression, Type returnType)
        {
            return new ExpressionRecorder(Expression.Constant(new ExpressionContainer(expression), returnType),
                BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
        }

        private static void ThrowMissingMemberException(Type type, string name)
        {
            throw new MissingMemberException(String.Format("Member named '{0}' not found on type '{1}'.", name, type));
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            return DoBindGetMember(binder.ReturnType, binder.Name, binder.IgnoreCase);
        }

        private DynamicMetaObject DoBindGetMember(Type returnType, string memberName, bool ignoreCase)
        {
            var wrapper = this.Value as ExpressionContainer;
            var valueExpr = wrapper.Expression;
            var property = Util.ResolveProperty(valueExpr.Type, memberName, ignoreCase, new object[0], !wrapper.IsStatic);
            if (property == null)
            {
                ThrowMissingMemberException(valueExpr.Type, memberName);
            }

            var memberExpr = Expression.Property(!wrapper.IsStatic ? valueExpr : null, property);

            return CreateRecorder(memberExpr, returnType);
        }
    }
}
