using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;

namespace iDai360.Scheduler.Service
{
    [RunInstaller(true)]
    public partial class SchedulerServiceInstaller : Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public SchedulerServiceInstaller()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = ConfigurationManager.AppSettings["ServiceName"];

            var serviceDescription = ConfigurationManager.AppSettings["ServiceDescription"];
            if (!string.IsNullOrEmpty(serviceDescription))
                serviceInstaller.Description = serviceDescription;

            var servicesDependedOn = new List<string> {  }; //默认服务依赖项
            var servicesDependedOnConfig = ConfigurationManager.AppSettings["ServicesDependedOn"]; //配置依赖项

            if (!string.IsNullOrEmpty(servicesDependedOnConfig))
                servicesDependedOn.AddRange(servicesDependedOnConfig.Split(new char[] { ',', ';' }));

            serviceInstaller.ServicesDependedOn = servicesDependedOn.ToArray();

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}