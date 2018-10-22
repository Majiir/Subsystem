using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Subsystem
{
    public class PropertyAccessor<T>
    {
        private Func<T> getter;
        private Action<T> setter;

        public T Get() => getter();
        public void Set(T value) => setter(value);
        public string Name { get; private set; }

        public PropertyAccessor(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("must be a MemberExpression", nameof(expression));
            }

            var instanceExpression = memberExpression.Expression;
            var parameter = Expression.Parameter(typeof(T), "obj");

            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException("must refer to a property", nameof(expression));
            }

            Name = propertyInfo.Name;

            getter = Expression.Lambda<Func<T>>(Expression.Call(instanceExpression, propertyInfo.GetGetMethod())).Compile();
            setter = Expression.Lambda<Action<T>>(Expression.Call(instanceExpression, propertyInfo.GetSetMethod(), parameter), parameter).Compile();
        }
    }
}
