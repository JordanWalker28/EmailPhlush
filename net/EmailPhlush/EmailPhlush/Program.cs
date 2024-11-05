using System;

namespace EmailPhlush
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("The application requires exactly 3 arguments: emai, password and method.");
                return;
            }

            string email = args[0];
            string password = args[1];
            string method = args[2];

            var emailService = new EmailService(AppConfig.ImapServer, AppConfig.Port);

            try
            {
                emailService.ConnectAndAuthenticate(email, password);

                if (method.Equals(JobType.Scan.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    emailService.ScanEmails(new DateTime(2024,11,3), new DateTime(2024,11,5));
                }else if (method.Equals(JobType.Delete.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    emailService.DeleteEmailsFromSender(AppConfig.SenderEmail);
                }
                else
                {
                    Console.WriteLine($"Job Type Not Recognised");
                }
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