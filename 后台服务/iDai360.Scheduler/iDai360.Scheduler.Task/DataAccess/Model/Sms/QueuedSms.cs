using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iDai360.Scheduler.Task.DataAccess.Model.Sms
{
    public class QueuedSms
    {
        public int Id { get; set; }

        public string Mobile { get; set; }

        public string Content { get; set; }

        public int SentTries { get; set; }

        public string Status { get; set; }

        public string StatusCode { get; set; }

        public DateTime? SentOnUtc { get; set; }
    }
}
