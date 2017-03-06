using System;
using System.Reflection;
using log4net;
using Quartz;

namespace iDai360.Scheduler.Core
{
    /// <summary>
    /// 任务抽象类
    /// </summary>
    [DisallowConcurrentExecution]
    public abstract class BaseTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                ExecuteTask(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Logger.Error(ex);
            }
        }

        public abstract void ExecuteTask(IJobExecutionContext context);

        /// <summary>
        /// 日志记录
        /// </summary>
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 格式化时间，最小时间过滤
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected static DateTime? FormatDateTime(DateTime? date)
        {
            if (date == null) return null;
            if (date.Value.ToString("yyyy-MM-dd") == "1900-01-01") return null;
            return date;
        }
    }
}
