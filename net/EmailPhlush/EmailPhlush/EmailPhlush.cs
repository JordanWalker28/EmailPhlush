namespace EmailPhlush;

public class EmailPhlush(string email, string password, string method)
{
    private readonly EmailService _emailService = new(AppConfig.ImapServer, AppConfig.Port);

    public void Execute()
    {
        try
        {
            _emailService.ConnectAndAuthenticate(email, password);

            if (method.Equals(JobType.Scan.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _emailService.ScanEmails(new DateTime(2024, 11, 3), DateTime.Now);
            }
            else if (method.Equals(JobType.Delete.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _emailService.DeleteEmailsFromSender(AppConfig.SenderEmail);
            }
            else
            {
                Console.WriteLine("Job Type Not Recognised");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            _emailService.Disconnect();
        }
    }
}