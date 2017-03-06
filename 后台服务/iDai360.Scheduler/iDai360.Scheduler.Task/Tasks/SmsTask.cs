using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iDai360.Scheduler.Core;
using iDai360.Scheduler.Task.DataAccess.Service;
using Quartz;

namespace iDai360.Scheduler.Task.Tasks
{
    /// <summary>
    /// 手机短信发送任务
    /// </summary>
    public class SmsTask : BaseTask
    {
        public override void ExecuteTask(IJobExecutionContext context)
        {
            const int maxTries = 3;
            var queuedSmses = SmsService.GetQueuedSmses(maxTries);
            var smsAccount = SmsService.GetSmsAccount();
            foreach (var queuedSms in queuedSmses)
            {
                try
                {
                    var result = SmsService.SendSms(smsAccount, queuedSms.Mobile, queuedSms.Content);
                    queuedSms.StatusCode = result.Code;
                    queuedSms.Status = result.Message;
                    if (result.Code == "100")
                    {
                        queuedSms.SentOnUtc = DateTime.UtcNow;
                    }
                }
                catch (Exception exc)
                {
                    Logger.Error(string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedSms.SentTries = queuedSms.SentTries + 1;
                    SmsService.UpdateQueuedSms(queuedSms);
                }
            }
        }
    }
}
