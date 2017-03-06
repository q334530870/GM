using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DKP.Core.Infrastructure;
using DKP.Services.Security;

namespace DKP.Web.Framework.Extensions
{
    public static class StringExtensions
    {
        public static string ToBankAccount(this string bankAccount)
        {
            if (string.IsNullOrEmpty(bankAccount) || bankAccount.Length < 4) return bankAccount;
            var times = 0;
            for (var i = 4; i < bankAccount.Length; i = i + 4)
            {
                if (i + times > bankAccount.Length) break;
                bankAccount = bankAccount.Insert(i + times, " ");
                times++;
            }
            return bankAccount;
        }

        public static string ToDicName(this string code, string groupCode)
        {
            return EngineContext.Current.Resolve<IDictionaryService>().GetNameByCode(groupCode, code);
        }

        public static string ToMoney(this decimal? amount, string defaultValue = "0.00")
        {
            if (amount == null) return defaultValue;
            return amount.Value.ToString("N");
        }

        public static string ToPercentage(this decimal? value, string defaultValue = "0.00", string format = "0.00")
        {
            return value == null ? defaultValue : (value*100).Value.ToString(format);
        }
    }
}
