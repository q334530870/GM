using System;
using System.Configuration;
using System.Reflection;
using iDai360.Scheduler.Core.Common;
using iDai360.Scheduler.Core.Configuration;
using log4net;
using Quartz;
using Quartz.Impl;

namespace iDai360.Scheduler.Core
{
    public static class SchedulerManager
    {
        /// <summary>
        /// Quartz Scheduler
        /// </summary>
        private static IScheduler _scheduler;
       
        /// <summary>
        /// 日志记录
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static SchedulerManager()
        {


        }

        /// <summary>
        /// 开启定时任务
        /// </summary>
        public static void Start()
        {
            //创建调度器
            ISchedulerFactory factory = new StdSchedulerFactory();
            _scheduler = factory.GetScheduler();

            var config = (SchedulerConfig)ConfigurationManager.GetSection("scheduler");
            foreach (Task task in config.Tasks)
            {
                if (task.Disabled) //任务处于关闭状态
                {
                    continue;
                }

                //通过反射获取Task类型
                Type taskType;
                Exception exception;
                if (!AssemblyUtil.TryGetType(task.Type, out taskType, out exception))
                {
                    Console.WriteLine("无法获取任务类型，" + task.Name + "!", exception);
                    Logger.Error("无法获取任务类型，" + task.Name + "!", exception);
                    continue;
                }

                var jobDetail = new JobDetailImpl(task.Name, taskType); //创建QuartzJob
                //创建触发器
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(task.Name + "_trigger")
                    .WithCronSchedule(task.Cron)
                    .Build();

                _scheduler.ScheduleJob(jobDetail, trigger);

            }

            _scheduler.Start();
            Logger.Info("定时任务服务开启成功！");
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public static void Stop()
        {
            if (_scheduler.IsStarted)
            {
                _scheduler.Shutdown(true); 
                Logger.Info("定时任务关闭成功！");
            }
        }
        
    }
}
