using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Core.Extensions
{
    public static class IntExtensions
    {
        public static int ToInt(this string val, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(val))
            {
                return defaultValue;
            }

            int outValue;
            int.TryParse(val, out outValue);
            return outValue;
        }
    }
}
