using Autofac;
using Autofac.Extensions.DependencyInjection;
using log4net;
using log4net.Appender;
using log4net.Config;
using Lys.Service.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeterKottas.DotNetCore.WindowsService;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lys.Portal
{
    public class MainHost
    {
        private readonly IServiceProvider m_Services;
        private readonly IConfiguration m_Config;
        private readonly ILog m_Logger;

        private string m_LogRepositoryName;
        private string m_EnvName;

        public MainHost()
        {
            m_Config = new ConfigurationBuilder()
                .SetBasePath(PortalConstants.Paths.BaseDir)
                .AddJsonFile("appsettings.json")
                .AddJsonFile(GetConfigFile())
                .Build();

            m_LogRepositoryName = m_Config["Log4Net:RepositoryName"];
            XmlConfigurator.Configure(LogManager.CreateRepository(m_LogRepositoryName), new FileInfo(PortalConstants.Paths.LogConfig));
            m_Logger = LogManager.GetLogger(m_LogRepositoryName, "ApplicationLogger");
            ConfigLog();

            m_Services = ConfigServices();
        }

        public void Run()
        {
            m_Logger.Info("启动程序");

            ServiceRunner<MainService>.Run(config =>
            {
                config.SetDescription(m_Config["Installer:Description"]);
                config.SetDisplayName(m_Config["Installer:DisplayName"]);
                config.SetName(m_Config["Installer:ServiceName"]);

                var name = config.GetDefaultName();

                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) => m_Services.GetRequiredService<MainService>());

                    serviceConfig.OnStart((service, extraArguments) =>
                    {
                        m_Logger.Info($"服务{name}启动，运行环境：{m_EnvName}");
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        m_Logger.Info($"服务{name}停止");
                        service.Stop();
                    });

                    serviceConfig.OnError(e =>
                    {
                        if (m_Logger == null)
                        {
                            File.AppendAllText(PortalConstants.Paths.ErrorLog, e + "\r\n");
                        }
                        else
                        {
                            m_Logger.Error(e);
                        }
                    });
                });
            });

            m_Logger.Info("程序执行完毕");
        }

        private void ConfigLog()
        {
            var res = LogManager.GetRepository(m_LogRepositoryName);
            var appenders = res.GetAppenders();
            foreach (var appender in appenders)
            {
                if (appender is RollingFileAppender roolingAppender)
                {
                    // 动态修改日志路径至程序运行目录
                    // 否则在安装windows服务后，会在服务运行目录（system32）下生成日志
                    roolingAppender.File = PortalConstants.Paths.LogDir;
                    roolingAppender.ActivateOptions();
                }
            }
        }

        private IServiceProvider ConfigServices()
        {
            var services = new ServiceCollection();
            
            services.AddDbContextPool<MySQLDbContext>(opt => opt.UseMySql(m_Config.GetConnectionString("MySQL")));

            services.AddSingleton<IConfiguration>(m_Config);
            services.AddSingleton<ILog>(m_Logger);
            services.AddSingleton<MainService>();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            var assemblies = "Service".Split(',')
                .Select(s => Assembly.Load($"Lys.{s}"))
                .ToArray();
            builder.RegisterAssemblyTypes(assemblies)
                .AsImplementedInterfaces()
                .AsSelf();

            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        private string GetConfigFile()
        {
            var envConfig = new ConfigurationBuilder()
                .SetBasePath(PortalConstants.Paths.BaseDir)
                .AddJsonFile("environment.json")
                .Build();

            m_EnvName = envConfig["Environment"];
            return string.IsNullOrEmpty(m_EnvName) ? "appsettings.json" : $"appsettings.{m_EnvName}.json";
        }
    }
}