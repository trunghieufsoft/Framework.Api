using System;
using Serilog;
using System.Linq;
using Entities.Entities;
using System.Reflection;
using Common.Core.Timing;
using Common.DTOs.Common;
using Entities.Enumerations;
using Common.Core.Extensions;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Service.Services.Abstractions
{
    public abstract class BaseService
    {
        protected readonly string _all = "All";
        protected readonly string _comma = ",";
        protected readonly int _maxLogin = 3;
        protected readonly int _randomStaff = 10;
        protected readonly int _randomManager = 5;
        protected virtual List<Expression<Func<T, bool>>> GetExpressions<T>(SearchInput input, int number)
        {
            if (input == null || input.KeySearch == null) return null;
            int indexCheck = 0;
            Expression expBody;
            string Key = string.Empty;
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
            List<Expression<Func<T, bool>>> listExpresion = new List<Expression<Func<T, bool>>>();
            foreach (KeyValuePair<string, string> item in input.KeySearch)
            {
                Key = item.Key.FirstCharToUpper();
                if (!string.IsNullOrEmpty(item.Value) && indexCheck <= number && (typeof(T).GetProperty(Key) != null))
                {
                    MemberExpression parameterExp = Expression.Property(parameterExpression, Key);
                    PropertyInfo propertyInfo = (PropertyInfo)parameterExp.Member;
                    Type propertyType = propertyInfo.PropertyType;

                    if (propertyType == typeof(string))
                    {
                        string value = item.Value.ToUpper();
                        string column = Key.ToLower();
                        if (input.SearchEqual.Any(e => e.ToLower().Equals(column)) && value.Split(_comma).Length > 1)
                        {
                            MethodInfo methodInfo = typeof(StringExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == "CompareAny");
                            expBody = Expression.Call(methodInfo, parameterExp, Expression.Constant(item.Value, typeof(string)));
                        }
                        else
                        {
                            MethodInfo methodContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            Expression left = Expression.Call(parameterExp, typeof(string).GetMethod("ToUpper", new Type[] { }));
                            Expression right = Expression.Constant(value, typeof(string));
                            expBody = Expression.Call(left, methodContains, right);
                        }
                    }
                    else if (propertyType == typeof(bool?))
                    {
                        expBody = Expression.Equal(parameterExp, Expression.Constant(true, propertyType));
                    }
                    else
                    {
                        var value = propertyType.IsEnum ? Enum.Parse(propertyType, item.Value.FirstCharToUpper()) : Int32.Parse(item.Value);
                        expBody = Expression.Equal(parameterExp, Expression.Constant(value, propertyType));
                    }
                    listExpresion.Add(Expression.Lambda<Func<T, bool>>(expBody, parameterExpression));
                }
                indexCheck++;
            }
            return listExpresion;
        }

        // Sort
        protected virtual IQueryable<T> ApplyOrderBy<T>(SearchInput input, IQueryable<T> data)
        {
            try
            {
                if (input == null || string.IsNullOrEmpty(input.OrderBy) || typeof(T).GetProperty(input.OrderBy.FirstCharToUpper()) == null)
                {
                    return data.OrderBy("LastUpdateDate desc");
                }
                if (input.IsSortDescending)
                {
                    return data.OrderBy(input.OrderBy + " desc");
                }

                return data.OrderBy(input.OrderBy + " asc");
            }
            catch (Exception e)
            {
                Log.Error("Cant not sort data {e}", e);
                return data;
            }
        }

        //Page
        protected virtual SearchOutput ApplyPaging(SearchInput input, IQueryable<object> data)
        {
            int total = 0;
            int limit = 0;
            int index = 0;
            IQueryable<object> paged = data;
            try
            {
                total = data != null ? data.Count() : 0;
                index = input != null && input.PageIndex != null && input.PageIndex > 0 ? input.PageIndex.Value : 0;
                limit = input != null && input.Limit > 0 ? input.Limit.Value : total;
                if (data != null && data.Count() > 0)
                {
                    paged = data.Skip(index * limit).Take(limit);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return new SearchOutput
            {
                TotalData = total,
                DataResult = paged,
                LimitData = limit,
                PageIndex = index
            };
        }

        public int CaculateDayOfConfig(SystemConfiguration obj)
        {
            try
            {
                if (obj == null)
                {
                    return 1;
                }
                if (obj.ValueUnit.Equals(Unit.days.ToString()))
                {
                    return Int32.Parse(obj.Value);
                }
                if (obj.ValueUnit.Equals(Unit.weeks.ToString()))
                {
                    return Int32.Parse(obj.Value) * 7;
                }
                if (obj.ValueUnit.Equals(Unit.months.ToString()))
                {
                    var totals = 0;
                    var value = Int32.Parse(obj.Value);
                    var current = Clock.Now;
                    for (int i = 0; i < value; i++)
                    {
                        int days = DateTime.DaysInMonth(current.Year, current.Month + i);
                        totals = totals + days;
                    }
                    return totals;
                }
            }
            catch (Exception e) { Log.Error("Something wrong when get system config {e}", e); }
            return 1;
        }

        public int CaculateMinutesOfConfig(SystemConfiguration obj)
        {
            try
            {
                if (obj == null)
                {
                    return 30;
                }
                if (obj.ValueUnit.Equals(Unit.days.ToString()))
                {
                    return Int32.Parse(obj.Value) * 1440;
                }
                if (obj.ValueUnit.Equals(Unit.weeks.ToString()))
                {
                    return Int32.Parse(obj.Value) * 7 * 1440;
                }
                if (obj.ValueUnit.Equals(Unit.minutes.ToString()))
                {
                    return Int32.Parse(obj.Value);
                }
                if (obj.ValueUnit.Equals(Unit.hour.ToString()))
                {
                    return Int32.Parse(obj.Value) * 60;
                }
                if (obj.ValueUnit.Equals(Unit.months.ToString()))
                {
                    var totals = 0;
                    var value = Int32.Parse(obj.Value);
                    var current = Clock.Now;
                    for (int i = 0; i < value; i++)
                    {
                        int days = DateTime.DaysInMonth(current.Year, current.Month + i);
                        totals = totals + days;
                    }
                    return totals * 1440;
                }
            }
            catch (Exception e) { Log.Error("Something wrong when get system config {e}", e); }
            return 30;
        }
    }
}