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

            var email = args[0];
            var password = args[1];
            var method = args[2];

            var serviceProvider = ConfigureServices(email, password, method);

            var emailJobProcessor = serviceProvider.GetRequiredService<IEmailPhlush>();
            emailJobProcessor.Execute();
        }

        private static IServiceProvider ConfigureServices(string email, string password, string method)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IWriter, Writer>();
            serviceCollection.AddSingleton<IEmailService>(_ =>
                new EmailService(AppConfig.ImapServer, AppConfig.Port));
            serviceCollection.AddSingleton<IEmailPhlush>(provider =>
                new Infrastructure.EmailPhlush(
                    provider.GetRequiredService<IWriter>(),
                    provider.GetRequiredService<IEmailService>(),
                    email, password, method));

            return serviceCollection.BuildServiceProvider();
        }
    }
}