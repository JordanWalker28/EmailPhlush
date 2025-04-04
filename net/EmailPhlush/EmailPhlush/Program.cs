using EmailPhlush.Application;
using EmailPhlush.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EmailPhlush
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("The application requires exactly 3 or more arguments: email, password, and method.");
                return;
            }

            IEmailConfig emailConfig = new EmailConfig(email:args[0], password:args[1], method:args[2]);

            var serviceProvider = ConfigureServices(emailConfig);

            var emailJobProcessor = serviceProvider.GetRequiredService<IEmailPhlush>();
            var serviceQuery = new ServiceQuery();
            emailJobProcessor.Execute(serviceQuery);
        }

        private static IServiceProvider ConfigureServices(IEmailConfig emailConfig)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IWriter, Writer>();
            serviceCollection.AddSingleton<IEmailService>(_ =>
                new EmailService(AppConfig.ImapServer, AppConfig.Port));
            serviceCollection.AddSingleton<IEmailPhlush>(provider =>
                new Infrastructure.EmailPhlush(
                    provider.GetRequiredService<IWriter>(),
                    provider.GetRequiredService<IEmailService>(),
                    emailConfig));

            return serviceCollection.BuildServiceProvider();
        }
    }
}