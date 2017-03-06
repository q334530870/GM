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
    public class ConditionalityExpiryCheckTask : BaseTask
    {
        public override void ExecuteTask(IJobExecutionContext context)
        {
            SubscriptionService.ConditionalityExpiryCheck();
        }
    }
}
