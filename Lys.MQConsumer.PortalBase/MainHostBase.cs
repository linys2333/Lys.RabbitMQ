using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lys.MQConsumer.PortalBase.Settings;
using Lys.MQConsumer.Service.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService;
using System;
using System.Reflection;

namespace Lys.MQConsumer.PortalBase
{
    public abstract class MainHostBase
    {
        protected readonly IServiceProvider m_Services;
        protected readonly IConfiguration m_Config;

        protected string m_EnvName;

        protected MainHostBase()
        {
            m_Config = new ConfigurationBuilder()
                .SetBasePath(PortalConstants.Paths.BaseDir)
                .AddJsonFile("appsettings.json")
                .AddJsonFile(GetConfigFile())
                .Build();

            m_Services = ConfigServices();

            var loggerFactory = m_Services.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            LogManager.LoadConfiguration("nlog.config");
        }

        public virtual void Run()
        {
            var logger = m_Services.GetRequiredService<ILogger<MainHostBase>>();
            logger.LogInformation($"启动程序，运行环境：{m_EnvName}");

            ServiceRunner<MainService>.Run(config =>
            {
                config.SetDescription(m_Config["Installer:Description"] + m_EnvName);
                config.SetDisplayName(m_Config["Installer:DisplayName"] + m_EnvName);
                config.SetName(m_Config["Installer:ServiceName"] + (string.IsNullOrEmpty(m_EnvName) ? "" : $".{m_EnvName}"));

                var name = config.GetDefaultName();

                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) => m_Services.GetRequiredService<MainService>());

                    serviceConfig.OnStart((service, extraArguments) =>
                    {
                        logger.LogInformation($"服务{name}启动，运行环境：{m_EnvName}");
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        logger.LogInformation($"服务{name}停止");
                        service.Stop();
                    });

                    serviceConfig.OnError(ex =>
                    {
                        logger.LogError(ex, "MQError");
                    });
                });
            });

            logger.LogInformation("程序执行完毕");
        }

        protected virtual IServiceProvider ConfigServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton(m_Config);
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            var logger = new LoggerFactory().CreateLogger("Lys.MQConsumer");
            services.AddSingleton(logger);

            services.AddSingleton(m_Config.GetSection("RabbitMQ").Get<RabbitMQSetting>());
            services.AddSingleton<MainService>();

            services.AddMemoryCache();
            services.AddDbContextPool<MySQLDbContext>(opt => opt.UseMySql(m_Config.GetConnectionString("MySQL")));

            RegisterServices(services);

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterAssemblyTypes(Assembly.Load("Lys.MQConsumer.Service"))
                .AsImplementedInterfaces()
                .AsSelf();

            RegisterServices(builder);

            return new AutofacServiceProvider(builder.Build());
        }

        protected virtual string GetConfigFile()
        {
            var envConfig = new ConfigurationBuilder()
                .SetBasePath(PortalConstants.Paths.BaseDir)
                .AddJsonFile("environment.json")
                .Build();

            m_EnvName = envConfig["Environment"];
            return string.IsNullOrEmpty(m_EnvName) ? "appsettings.json" : $"appsettings.{m_EnvName}.json";
        }

        protected abstract void RegisterServices(ServiceCollection services);

        protected abstract void RegisterServices(ContainerBuilder builder);
    }
}