using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ReduxReactive
{
    public static class ImmutableExtensions
    {
        public static TSource Clone<TSource>(this TSource source) where TSource : class, new()
        {
            try
            {
                TSource result = new TSource();
                foreach (var property in typeof(TSource).GetProperties())
                {
                    try
                    {
                        if (property.CanWrite)
                            property.SetValue(result, property.GetValue(source));
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                throw;
            }
        }

        public static TSource With<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> selector, TProperty value) where TSource : class, new()
        {
            var property = selector.PropertyInfo();

            if (!property.CanWrite)
                throw new ArgumentException(string.Format("Expression '{0}' refers a readonly property.", source.ToString()));

            property.SetValue(source, value);

            return source;
        }

        public static PropertyInfo PropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> source)
        {
            Type type = typeof(TSource);

            MemberExpression member = source.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", source.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", source.ToString()));

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.", source.ToString(), type));

            return propInfo;
        }

        public static IEnumerable<TSource> AsEnumerable<TSource>(this TSource source)
        {
            yield return source;
        }
    }
}
