namespace EmailPhlush.Infrastructure;

public class EmailPhlush(IEmailService emailService, string email, string password, string methodOfUse)
    : IEmailPhlush
{
    public void Execute()
    {
        try
        {
            emailService.ConnectAndAuthenticate(email, password);

            switch (methodOfUse.ToLower())
            {
                case var method when method.Equals(JobType.Scan.ToString().ToLower()):
                    emailService.ScanEmails(new DateTime(2024, 11, 3), DateTime.Now);
                    break;
    
                case var method when method.Equals(JobType.Delete.ToString().ToLower()):
                    var emailList = new List<string>()
                    {
                        "uk@marketing.axs.com",
                        "newsletters-noreply@linkedin.com",
                        "no-reply@harringtonstarr.com"
                    };
                    emailService.DeleteEmailsFromSender(emailList);
                    break;

                default:
                    Console.WriteLine("Job Type Not Recognised");
                    break;
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