using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKP.Core.Extensions
{
    public static class DecimalExtensions
    {
        public static float ToFloat(this decimal? dec)
        {
            if (dec == null) return 0;

            return (float)dec.Value;
        }
        public static float ToFloat(this decimal dec)
        {
            return (float)dec;
        }

        public static decimal ToDecimal(this string val, decimal defaultValue = 0)
        {
            if (string.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            decimal outValue;
            decimal.TryParse(val, out outValue);
            return outValue;
        }

        public static decimal ToDecimal(this double val,  decimal defaultValue = 0)
        {
            return Convert.ToDecimal(val);
        }

        public static string ToMonay(this string value)
        {
            if (string.IsNullOrEmpty(value)) { return "0.00"; }
            try { return Convert.ToDecimal(value).ToString("N"); }
            catch {
                return "0.00";
            }
        }

        public static decimal ToValue(this decimal? val, decimal defaultValue = 0)
        {
            return val ?? defaultValue;
        }
    }
}
