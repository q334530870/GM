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
    /// 确认认购单，T1转T0
    /// </summary>
    public class ConfirmSubscriptionTask : BaseTask
    {
        public override void ExecuteTask(IJobExecutionContext context)
        {
            SubscriptionService.Confirm();
        }
    }
}
