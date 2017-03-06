using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Dapper;
using iDai360.Scheduler.Task.Common;
using iDai360.Scheduler.Task.DataAccess.Model.Email;
using log4net;
using log4net.Repository.Hierarchy;

namespace iDai360.Scheduler.Task.DataAccess.Service
{
    public static class EmailService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static EmailAccount _emailAccount;

        public static List<QueuedEmail> GetQueuedEmails(int maxTries)
        {
            using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
            {
                return
                    conn.Query<QueuedEmail>(
                        @"select top 20 [Id],[PriorityId],[From],[FromName],[To],[ToName],[ReplyTo],[ReplyToName],[CC],[Bcc],
                                 [Subject],[Body],[AttachmentFilePath],[AttachmentFileName],[AttachedDownloadId],
                                 [SentTries],[SentOnUtc] 
                        from QueuedEmail 
                        where SentOnUtc is null and SentTries < @maxTries 
                        order by PriorityId desc,CreateTime asc ",
                        new { maxTries }).ToList();
            }
        }

        public static void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
            {
                conn.Execute("update QueuedEmail set SentOnUtc=@SentOnUtc,SentTries=@SentTries where id = @id",
                    queuedEmail);
            }
        }

        public static EmailAccount GetEmailAccount()
        {
            if (_emailAccount != null) return _emailAccount;
            try
            {
                using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
                {
                    var result = conn.Query("select * from Setting where Name like 'emailaccount%'").ToList();
                    var displayname = result.FirstOrDefault(t => t.Name == "emailaccount.displayname").Value ?? "";
                    var host = result.FirstOrDefault(t => t.Name == "emailaccount.host").Value ?? "";
                    var port = Convert.ToInt32((result.FirstOrDefault(t => t.Name == "emailaccount.port").Value ?? "0"));
                    var username = result.FirstOrDefault(t => t.Name == "emailaccount.username").Value ?? "";
                    var password = result.FirstOrDefault(t => t.Name == "emailaccount.password").Value ?? "";
                    var enableSsl = Convert.ToInt32(result.FirstOrDefault(t => t.Name == "emailaccount.enablessl").Value ?? "");
                    var useDefaultCredentials = Convert.ToBoolean(
                        Convert.ToInt32(
                            result.FirstOrDefault(t => t.Name == "emailaccount.usedefaultcredentials").Value ?? ""));

                    var email = result.FirstOrDefault(t => t.Name == "emailaccount.email").Value ?? "";
                    var emailAccount = new EmailAccount
                    {
                        Email = email,
                        DisplayName = displayname,
                        Host = host,
                        Port = port,
                        Username = username,
                        Password = password,
                        EnableSsl = enableSsl == 1,
                        UseDefaultCredentials = useDefaultCredentials
                    };

                    _emailAccount = emailAccount;

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return _emailAccount;
        }

        public static void SendEmail(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
             string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            int attachedDownloadId = 0)
        {
            var message = new MailMessage();
            //from, to, reply to
            message.From = new MailAddress(fromAddress, fromName);
            message.To.Add(new MailAddress(toAddress, toName));
            if (!String.IsNullOrEmpty(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
            }

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            //CC
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            //content
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            //create  the file attachment for this e-mail message
            if (!String.IsNullOrEmpty(attachmentFilePath) &&
                File.Exists(attachmentFilePath))
            {
                var attachment = new Attachment(attachmentFilePath);
                attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentFilePath);
                attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentFilePath);
                attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentFilePath);
                if (!String.IsNullOrEmpty(attachmentFileName))
                {
                    attachment.Name = attachmentFileName;
                }
                message.Attachments.Add(attachment);
            }
            //another attachment?
            if (attachedDownloadId > 0)
            {
                // 暂时不需要附件
                //var download = _downloadService.GetDownloadById(attachedDownloadId);
                //if (download != null)
                //{
                //    //we do not support URLs as attachments
                //    if (!download.UseDownloadUrl)
                //    {
                //        string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
                //        fileName += download.Extension;


                //        var ms = new MemoryStream(download.DownloadBinary);
                //        var attachment = new Attachment(ms, fileName);
                //        //string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                //        //var attachment = new Attachment(ms, fileName, contentType);
                //        attachment.ContentDisposition.CreationDate = DateTime.UtcNow;
                //        attachment.ContentDisposition.ModificationDate = DateTime.UtcNow;
                //        attachment.ContentDisposition.ReadDate = DateTime.UtcNow;
                //        message.Attachments.Add(attachment);
                //    }
                //}
            }

            //send email
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.EnableSsl;
                if (emailAccount.UseDefaultCredentials)
                    smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                else
                    smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);
                smtpClient.Send(message);
            }
        }
    }
}
