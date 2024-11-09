using System;
using Microsoft.Extensions.DependencyInjection;

namespace EmailPhlush
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("The application requires exactly 3  or more arguments: email, password, and method.");
                return;
            }

            var email = args[0];
            var password = args[1];
            var method = args[2];
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IEmailService>(_ => new EmailService(AppConfig.ImapServer, AppConfig.Port))
                .AddSingleton<IEmailPhlush>(provider => new EmailPhlush(provider.GetRequiredService<IEmailService>(), email, password, method))
                .BuildServiceProvider();

            var emailJobProcessor = serviceProvider.GetRequiredService<IEmailPhlush>();
            emailJobProcessor.Execute();
        }
    }
}