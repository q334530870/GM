using System;
using System.Data;
using System.Globalization;

namespace DKP.Core.Extensions
{
    /// <summary>
    /// 时间处理扩展类
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// 转换成详细时间格式 2015-01-01 12:00
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 转换成详细时间格式 2015-01-01 12:00
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime? dateTime)
        {
            return dateTime == null ? "" : dateTime.Value.ToDateTimeString();
        }

        public static string ToDateTimeString(this DateTime? dateTime, string defaultValue)
        {
            return dateTime == null ? defaultValue : dateTime.Value.ToDateTimeString();
        }


        /// <summary>
        /// 转换成日期格式 2015-01-01
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 转换成日期格式 2015-01-01
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime? dateTime, string defaultValue = "")
        {
            return dateTime == null ? defaultValue : dateTime.Value.ToDateString();
        }

        /// <summary>
        /// 将字符串转换成时间类型
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString)) return DateTime.Now;

            //DateTime dt;
            //if (!DateTime.TryParse(dateTimeString, out dt))
            //{
            //    return DateTime.Now;
            //}
            //return dt;
            return Convert.ToDateTime(dateTimeString);
        }

        /// <summary>
        /// 将字符串转换成时间类型
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateString(this string dateTimeString, string format = "yyyy-MM-dd")
        {
            if (string.IsNullOrEmpty(dateTimeString)) return "";
            return Convert.ToDateTime(dateTimeString).ToString(format);
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <returns></returns>
        public static DateTime ToDateTime(this double timestamp)
        {
            DateTime converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime newDateTime = converted.AddSeconds(timestamp);
            return newDateTime.ToLocalTime();
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <returns></returns>
        public static double ToTimestamp(this DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return span.TotalSeconds;
        }
    }
}
