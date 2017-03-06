using System.ServiceProcess;
using iDai360.Scheduler.Core;

namespace iDai360.Scheduler.Service
{
    partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
           SchedulerManager.Start();
        }

        protected override void OnStop()
        {
            SchedulerManager.Stop();
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            SchedulerManager.Stop();
            base.OnShutdown();
        }
    }
}
