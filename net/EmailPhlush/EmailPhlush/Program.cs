using System;

namespace EmailPhlush
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("The application requires exactly 2 arguments: email and password.");
                return;
            }

            string email = args[0];
            string password = args[1];

            var emailService = new EmailService(AppConfig.ImapServer, AppConfig.Port);

            try
            {
                emailService.ConnectAndAuthenticate(email, password);
                emailService.DeleteEmailsFromSender(AppConfig.SenderEmail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                emailService.Disconnect();
            }
        }
    }
}