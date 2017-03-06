using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DKP.Core;
using DKP.Core.Infrastructure;
using DKP.Services.Security;

namespace DKP.Web.Framework.Extensions
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// 根据字典组code和字典code获取字典名称
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="groupCode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ToDicName(this HtmlHelper htmlHelper, string groupCode, string code)
        {
            return EngineContext.Current.Resolve<IDictionaryService>().GetNameByCode(groupCode, code);
        }

        /// <summary>
        /// 手机号隐藏中间4位
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static string ToSafeMobile(this HtmlHelper htmlHelper, string mobile)
        {
            if (string.IsNullOrEmpty(mobile) || mobile.Length < 11) return mobile;
            string result = string.Empty;
            var charArray = mobile.ToCharArray();
            charArray[4] = '*';
            charArray[5] = '*';
            charArray[6] = '*';
            charArray[7] = '*';
            return charArray.Aggregate(result, (current, item) => current + item);
        }
        public static string ToSafeIdNumber(this HtmlHelper htmlHelper, string idNumber)
        {
            if (string.IsNullOrEmpty(idNumber) || idNumber.Length < 14) return idNumber;
            string result = string.Empty;
            var charArray = idNumber.ToCharArray();
            for (var i = 0; i < 14; i++)
            {
                charArray[i] = '*';
            }
            return charArray.Aggregate(result, (current, item) => current + item);
        }
        public static string ToSafeBankAccount(this HtmlHelper htmlHelper, string bankAccount)
        {
            if (string.IsNullOrEmpty(bankAccount) || bankAccount.Length < 15) return bankAccount;
            string result = string.Empty;
            var charArray = bankAccount.ToCharArray();
            for (var i = 4; i < 13; i++)
            {
                charArray[i] = '*';
            }
            return charArray.Aggregate(result, (current, item) => current + item);
        }

        public static string ShowUserName(this HtmlHelper htmlHelper)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var name = workContext.CurrentUser.RealName;
            if (string.IsNullOrEmpty(name))
            {
                name = workContext.CurrentUser.Name;
            }
            return name;
        }

        public static MvcHtmlString NoData<T>(this HtmlHelper htmlHelper, IList<T> list, int rowCount)
        {
            var html = string.Empty;
            if (list == null || list.Count == 0)
            {
                html = "<tr>" +
                       "    <td colspan='" + rowCount + "' class='center'>" +
                       "        <span class='gray'><i class='fa fa-warning fa-2x'></i> 没有相关数据</span>" +
                       "    </td>" +
                       "</tr>";
            }
            return new MvcHtmlString(html);
        }
    }
}
