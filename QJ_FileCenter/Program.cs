﻿using glTech.Log4netWrapper;
using Nancy.Hosting.Self;
using QJ_FileCenter;
using QJ_FileCenter.Domains;
using QJ_FileCenter.Repositories;
using QJFile.Data;
using System;
using System.Net;
using System.ServiceProcess;

namespace QJ_FileCenter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Logger.Initialize(PathUtil.GetLog4netPath());
            Logger.LogError("开始");
            string strAppType = CommonHelp.GetConfig("apptype","0");
            if (strAppType == "0")
            {
                try
                {
                    var hostConfiguration = new HostConfiguration
                    {
                        UrlReservations = new UrlReservations() { CreateAutomatically = true }
                    };
                    string strIP = appsetingB.GetValueByKey("ip");
                    string port = appsetingB.GetValueByKey("port");

                    string url = string.Format("http://{0}:{1}", strIP, port);
                    var rootPath = appsetingB.GetValueByKey("path");
                    var nancyHost = new NancyHost(new RestBootstrapper(), hostConfiguration, new Uri(url));
                    nancyHost.Start();
                    System.Console.WriteLine("文件中心服务开启,管理地址:" + url.ToString());

                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Logger.LogError("启动NancyHost失败.");
                    Logger.LogError4Exception(ex);
                }
            }
            else
            {
                if (!Environment.UserInteractive)
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                    new QJ_FileCenterService()
                    };
                    ServiceBase.Run(ServicesToRun);
                }
                else
                {
                    QJ_FileCenterService service = new QJ_FileCenterService();
                    System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);  // forces debug to keep VS running while we debug the service  
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null && e.ExceptionObject != null)
            {
                Logger.LogError("未捕获异常");
                Logger.LogError4Exception((Exception)e.ExceptionObject);
            }
        }
    }
}
