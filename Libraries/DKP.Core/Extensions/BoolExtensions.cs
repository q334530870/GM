using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Core.Extensions
{
    public static class BoolExtensions
    {
        public static string ToString(this bool val)
        {
            return val ? "是" : "否";
        }

        public static string ToSubmitString(this bool val)
        {
            return val ? "已提交" : "未提交";
        }

        public static string ToCustomString(this bool val, string str)
        {
            var arr = str.Split('|');
            return val ? arr[0] : arr[1];
        }

        public static string ToCustomString(this int val, string str)
        {
            var arr = str.Split('|');
            return val == 1 ? arr[0] : arr[1];
        }
    }
}
