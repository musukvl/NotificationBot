using Amba.ImageTools.Infrastructure;
using NotificationBot.Infrastructure;

namespace NotificationBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            var applicationBuilder = new ApplicationBuilder<Application>();
            var application = applicationBuilder
                .ReadConfiguration()
                .RegisterServices((serviceCollection, configuration) => Application.ConfigureServices(serviceCollection, configuration))
                .Build();
            application.Run(args);
        }        
    }
}