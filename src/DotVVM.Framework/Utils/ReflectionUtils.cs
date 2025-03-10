using DotVVM.Framework.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace DotVVM.Framework.Utils
{
    public class ReflectionUtils
    {

        /// <summary>
        /// Gets the property name from lambda expression, e.g. 'a => a.FirstName'
        /// </summary>
        public static string GetPropertyNameFromExpression<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                var unaryExpressionBody = (UnaryExpression)expression.Body;
                body = unaryExpressionBody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        /// <summary>
        /// Gets the specified property of a given object.
        /// </summary>
        public static object GetObjectProperty(object item, string propertyName)
        {
            if (item == null) return null;

            var type = item.GetType();
            var prop = type.GetProperty(propertyName);
            if (prop == null)
            {
                throw new Exception(String.Format("The object of type {0} does not have a property named {1}!", type, propertyName));     // TODO: exception handling
            }
            return prop.GetValue(item);
        }

        /// <summary>
        /// Extracts the value of a specified property and converts it to string. If the property name is empty, returns a string representation of a given object.
        /// Null values are converted to empty string.
        /// </summary>
        public static string ExtractMemberStringValue(object item, string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                item = GetObjectProperty(item, propertyName);
            }
            return item != null ? item.ToString() : "";
        }


        /// <summary>
        /// Converts a value to a specified type
        /// </summary>
        public static object ConvertValue(object value, Type type)
        {
            // handle null values
            if ((value == null) && (type.IsValueType))
                return Activator.CreateInstance(type);

            // handle nullable types
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if ((value is string) && ((string)value == string.Empty))
                {
                    // value is an empty string, return null
                    return null;
                }
                else
                {
                    // value is not null
                    var nullableConverter = new NullableConverter(type);
                    type = nullableConverter.UnderlyingType;
                }
            }

            // handle exceptions
            if ((value is string) && (type == typeof(Guid)))
                return new Guid((string)value);
            if (type == typeof(object)) return value;

            // handle enums
            if (type.IsEnum && value is string)
            {
                try
                {
                    return Enum.Parse(type, (string)value);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("The enum {0} does not allow a value '{1}'!", type, value), ex);      // TODO: exception handling
                }
            }

            // generic to string
            if (type == typeof(string))
            {
                return value.ToString();
            }

            // convert
            return Convert.ChangeType(value, type);
        }

        public static Type GetEnumerableType(Type collectionType)
        {
            // handle array
            if (collectionType.IsArray)
            {
                return collectionType.GetElementType();
            }

            // handle iEnumerables
            Type iEnumerable;
            if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
            {
                iEnumerable = collectionType;
            }
            else
            {
                iEnumerable = collectionType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEnumerable<>));
            }
            if (iEnumerable != null)
            {
                return iEnumerable.GetGenericArguments()[0];
            }

            // handle GridViewDataSet
            if (typeof(IGridViewDataSet).IsAssignableFrom(collectionType))
            {
                var itemsType = collectionType.GetProperty(nameof(IGridViewDataSet.Items)).PropertyType;
                return GetEnumerableType(itemsType);
            }

            // handle object collections
            if (typeof (IEnumerable).IsAssignableFrom(collectionType))
            {
                return typeof(object);
            }

            throw new NotSupportedException();      // TODO
        }
    }
}
