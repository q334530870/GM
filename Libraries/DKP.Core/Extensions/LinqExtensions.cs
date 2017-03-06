using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Dynamic;
using DKP.Core.Models;

namespace DKP.Core.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// 解析form表单成linq表达式
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        public static string ResolveToLinq(this NameValueCollection forms)
        {
            string result = " 1 = 1 ";

            int i = 0;
            foreach (string key in forms)
            {
                var str = key.Split('_');

                if (string.IsNullOrEmpty(forms[key]) || str.Length != 3) continue;

                result += " and ";

                var value = forms[key].Trim();
                switch (str[1].ToUpper())
                {
                    case "EQ":
                        result += string.Format("{0}=\"{1}\"", str[2], value);
                        break;
                    case "NEQ":
                        result += string.Format("{0} <> \"{1}\"", str[2], value);
                        break;
                    case "EQBOOL":
                        result += string.Format("{0}={1}", str[2], bool.Parse(value));
                        break;
                    case "NEQBOOL":
                        result += string.Format("{0}!={1}", str[2], bool.Parse(value));
                        break;
                    case "EQINT":
                        result += string.Format("{0}={1}", str[2], value);
                        break;
                    case "NEQINT":
                        result += string.Format("{0}<>{1}", str[2], value);
                        break;
                    case "LIKE":
                        result += string.Format("{0}.Contains(\"{1}\")", str[2], value);
                        break;
                    case "GT":
                        result += string.Format("{0}>\"{1}\"", str[2], value);
                        break;
                    case "LT":
                        result += string.Format("{0}<\"{1}\"", str[2], value);
                        break;
                    case "GTE":
                        result += string.Format("{0}>=\"{1}\"", str[2], value);
                        break;
                    case "LTE":
                        result += string.Format("{0}<=\"{1}\"", str[2], value);
                        break;
                    case "GTTIME":
                        var date = Convert.ToDateTime(value);
                        result += string.Format("{0}>DateTime({1}, {2}, {3}, {4}, {5}, {6})",
                            str[2], date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                        break;
                    case "LTTIME":
                        date = Convert.ToDateTime(value);
                        result += string.Format("{0}<DateTime({1}, {2}, {3}, {4}, {5}, {6})",
                            str[2], date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                        break;
                    case "GTETIME":
                        date = Convert.ToDateTime(value);
                        result += string.Format("{0}>=DateTime({1}, {2}, {3}, {4}, {5}, {6})",
                            str[2], date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                        break;
                    case "LTETIME":
                        date = Convert.ToDateTime(value);
                        result += string.Format("{0}<=DateTime({1}, {2}, {3}, {4}, {5}, {6})",
                            str[2], date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                        break;
                }
                i++;
            }

            return result;
        }
    }
}
