using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Common.Core.Timing;

namespace Database.EntityFrameworkCore
{
    public class EntityMaterializerSource : Microsoft.EntityFrameworkCore.Metadata.Internal.EntityMaterializerSource
    {
        private static readonly MethodInfo NormalizeMethod =
            typeof(Clock).GetTypeInfo().GetMethod(nameof(Clock.Normalize));

        private static readonly MethodInfo NormalizeNullableMethod =
            typeof(Clock).GetTypeInfo().GetMethod(nameof(Clock.NormalizeNullable));

        private static readonly MethodInfo NormalizeObjectMethod =
            typeof(Clock).GetTypeInfo().GetMethod(nameof(Clock.NormalizeObject));

        public override Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, IPropertyBase property)
        {
            if (type == typeof(DateTime))
            {
                return Expression.Call(
                    NormalizeMethod,
                    base.CreateReadValueExpression(valueBuffer, type, index, property));
            }

            if (type == typeof(DateTime?))
            {
                return Expression.Call(
                    NormalizeNullableMethod,
                    base.CreateReadValueExpression(valueBuffer, type, index, property));
            }

            if (type == typeof(object))
            {
                return Expression.Call(
                    NormalizeObjectMethod,
                    base.CreateReadValueExpression(valueBuffer, type, index, property));
            }

            return base.CreateReadValueExpression(valueBuffer, type, index, property);
        }
    }
}