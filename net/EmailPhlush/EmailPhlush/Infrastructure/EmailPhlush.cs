namespace EmailPhlush.Infrastructure;

public class EmailPhlush : IEmailPhlush
{
    private readonly IEmailService _emailService;
    private readonly string _email;
    private readonly string _password;
    private readonly string _method;

    public EmailPhlush(IEmailService emailService, string email, string password, string method)
    {
        _emailService = emailService;
        _email = email;
        _password = password;
        _method = method;
    }

    public void Execute()
    {
        try
        {
            _emailService.ConnectAndAuthenticate(_email, _password);

            if (_method.Equals(JobType.Scan.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _emailService.ScanEmails(new DateTime(2024, 11, 3), DateTime.Now);
            }
            else if (_method.Equals(JobType.Delete.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                //_emailService.DeleteEmailsFromSender(AppConfig.SenderEmail);
                var emailList = new List<string>()
                {
                    "uk@marketing.axs.com",
                    "newsletters-noreply@linkedin.com",
                    "no-reply@harringtonstarr.com"
                };
                
                _emailService.DeleteEmailsFromSender(emailList);
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