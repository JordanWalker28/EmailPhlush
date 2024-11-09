using Microsoft.Extensions.Configuration;

namespace EmailPhlush.Application
{
    public static class AppConfig
    {
        public static readonly string ImapServer;
        public static readonly int Port;
        public static readonly string SenderEmail;

        static AppConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ImapServer = configuration["EmailSettings:ImapServer"];
            Port = int.Parse(configuration["EmailSettings:Port"]);
            SenderEmail = configuration["EmailSettings:SenderEmail"];
        }
    }
}