using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKP.Core.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// [优化]支持Nullable数据类型的转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ChangeType(this Type conversionType, object value)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value is String && string.IsNullOrEmpty(value.ToString()))
                {
                    return null;
                }

                if (value != null)
                {
                    var nullableConverter = new NullableConverter(conversionType);
                    conversionType = nullableConverter.UnderlyingType;
                }
                else
                {
                    return null;
                }
            }

            return Convert.ChangeType(value, conversionType);
        }
    }
}
