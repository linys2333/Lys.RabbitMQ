using Autofac;
using Lys.MQConsumer.PortalBase;
using Microsoft.Extensions.DependencyInjection;

namespace Lys.MQConsumer.Sample.Portal
{
    public class MainHost : MainHostBase
    {
        protected override void RegisterServices(ServiceCollection services)
        {
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<MainHandler>().AsImplementedInterfaces().AsSelf();
        }
    }
}