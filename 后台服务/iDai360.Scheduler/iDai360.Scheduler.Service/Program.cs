using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using iDai360.Scheduler.Core;
using log4net;
using log4net.Config;
using Quartz;

namespace iDai360.Scheduler.Service
{
    static partial class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //初始化log4net配置文件
            var config = Path.Combine("Configs", "log4net.config");
            if (!Path.IsPathRooted(config))
                config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config);
            XmlConfigurator.Configure(new FileInfo(config));

            try
            {
                #region run service

                if (args != null && args.Length > 0)
                {
                    if (args[0].Equals("-i", StringComparison.OrdinalIgnoreCase))
                    {
                        SelfInstaller.InstallMe();
                        return;
                    }
                    else if (args[0].Equals("-u", StringComparison.OrdinalIgnoreCase))
                    {
                        SelfInstaller.UninstallMe();
                        return;
                    }
                    else if (args[0].Equals("-c", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine();
                        Console.WriteLine("按任意键启动服务！");
                        Console.ReadKey();
                        Console.WriteLine();
                        RunAsConsole();
                    }
                    else
                    {
                        Console.WriteLine(args[0]);
                    }
                }
                else
                {
                    RunAsService();
                }

                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
            }
        }

        static void RunAsConsole()
        {
            try
            {

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：系统初始化中...");
                SchedulerManager.Start();
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：启动成功...");
                Console.WriteLine();
                Console.WriteLine("请按'q'键退出系统！");

                while (Console.ReadKey().Key != ConsoleKey.Q)
                {
                    Console.WriteLine();
                }

                Console.WriteLine();

                SchedulerManager.Stop();

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：系统已经关闭！");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.Write("按任意键退出...");
                Console.ReadKey();
            }
        }

        static void RunAsService()
        {
            var services = new ServiceBase[] { new MainService() };
            ServiceBase.Run(services);
        }
    }
}