using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using iDai360.Scheduler.Core.Common;
using iDai360.Scheduler.Task.Common;
using iDai360.Scheduler.Task.DataAccess.Model.Sms;
using log4net;

namespace iDai360.Scheduler.Task.DataAccess.Service
{
    public class SmsService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static SmsAccount _smsAccount;

        public static SmsAccount GetSmsAccount()
        {
            if (_smsAccount == null)
            {
                using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
                {
                    var result = conn.Query("select * from Setting where Name like 'smsaccount%'").ToList();
                    var smsAccount = new SmsAccount
                    {
                        Url = result.FirstOrDefault(t => t.Name == "smsaccount.url").Value,
                        UserName = result.FirstOrDefault(t => t.Name == "smsaccount.username").Value,
                        Password = result.FirstOrDefault(t => t.Name == "smsaccount.password").Value
                    };

                    _smsAccount = smsAccount;
                }
            }

            return _smsAccount;
        }

        public static void UpdateQueuedSms(QueuedSms queuedSms)
        {
            using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
            {
                conn.Execute("update QueuedSms set SentOnUtc=@SentOnUtc,SentTries=@SentTries,StatusCode=@StatusCode,Status=@Status where id = @id",
                    queuedSms);
            }
        }

        public static List<QueuedSms> GetQueuedSmses(int maxTries)
        {
            using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
            {
                return
                    conn.Query<QueuedSms>(
                        @"select [Id],[Mobile],[Content],[SentTries],[SentOnUtc],[Status],[StatusCode]
                        from QueuedSms 
                        where SentOnUtc is null and SentTries < @maxTries 
                        order by CreateTime ",
                        new { maxTries }).ToList();
            }
        }

        public static SmsSendResult SendSms(SmsAccount smsAccount, string mobile, string content)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string pwd =
                System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(
                    smsAccount.Password + smsAccount.UserName, "MD5");
            string mobileids = mobile + Convert.ToInt64(ts.TotalSeconds);

            var url = string.Format("?uid={0}&pwd={1}&mobile={2}&mobileids={3}&content={4}", smsAccount.UserName, pwd,
                mobile, mobileids, System.Web.HttpUtility.UrlEncode(content));
            var result = HttpUtil.Get(smsAccount.Url + url);

            var s = result.Split('&');

            return new SmsSendResult
            {
                Code = s[1].Split('=')[1],
                Message = s[2].Split('=')[1]
            };
        }
    }
}
