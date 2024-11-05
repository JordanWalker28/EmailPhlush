namespace EmailPhlush;

public class EmailPhlush
{
    private readonly string _email;
    private readonly string _password;
    private readonly string _method;
    private readonly EmailService _emailService;

    public EmailPhlush(string email, string password, string method)
    {
        _email = email;
        _password = password;
        _method = method;
        _emailService = new EmailService(AppConfig.ImapServer, AppConfig.Port);
    }

    public void Execute()
    {
        try
        {
            _emailService.ConnectAndAuthenticate(_email, _password);

            if (_method.Equals(JobType.Scan.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _emailService.ScanEmails(new DateTime(2024, 11, 3), new DateTime(2024, 11, 5));
            }
            else if (_method.Equals(JobType.Delete.ToString(), StringComparison.OrdinalIgnoreCase))
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