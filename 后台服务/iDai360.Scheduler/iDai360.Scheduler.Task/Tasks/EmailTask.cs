using System;
using System.Threading;
using iDai360.Scheduler.Core;
using iDai360.Scheduler.Task.DataAccess.Service;
using Quartz;

namespace iDai360.Scheduler.Task.Tasks
{
    /// <summary>
    /// 电子邮件发送任务
    /// </summary>
    public class EmailTask : BaseTask
    {
        public override void ExecuteTask(IJobExecutionContext context)
        {
            const int maxTries = 3;
            var queuedEmails = EmailService.GetQueuedEmails(maxTries);
            var emailAccount = EmailService.GetEmailAccount();
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = String.IsNullOrWhiteSpace(queuedEmail.Bcc)
                            ? null
                            : queuedEmail.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = String.IsNullOrWhiteSpace(queuedEmail.CC)
                            ? null
                            : queuedEmail.CC.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    Thread.Sleep(500);

                    EmailService.SendEmail(emailAccount,
                        queuedEmail.Subject,
                        queuedEmail.Body,
                        queuedEmail.From,
                        queuedEmail.FromName,
                        queuedEmail.To,
                        queuedEmail.ToName,
                        queuedEmail.ReplyTo,
                        queuedEmail.ReplyToName,
                        bcc,
                        cc,
                        queuedEmail.AttachmentFilePath,
                        queuedEmail.AttachmentFileName,
                        queuedEmail.AttachedDownloadId);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    Logger.Error(string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedEmail.SentTries = queuedEmail.SentTries + 1;
                    EmailService.UpdateQueuedEmail(queuedEmail);
                }
            }
        }
    }
}
