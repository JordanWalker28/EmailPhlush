namespace EmailPhlush.Infrastructure;

public class EmailPhlush(IEmailService emailService, string email, string password, string methodOfUse)
    : IEmailPhlush
{


    public class ServiceQuery()
    {
        public string emailSenderToRemove = "em@mail.totallymoney.com";
        public DateTime dateTimeFrom = new(2024, 11, 3);
        public DateTime dateTimeTo =  DateTime.Now;
        public List<string> emailSendersToRemove = ["contact@mailer.humblebundle.com"];
        
    }
    
    public void Execute()
    {
        var serviceQuery = new ServiceQuery();
        try
        {
            emailService.ConnectAndAuthenticate(email, password);

            switch (methodOfUse.ToLower())
            {
                case var method when method.Equals(JobType.Scan.ToString().ToLower()):
                    emailService.ScanEmails(serviceQuery.dateTimeFrom, serviceQuery.dateTimeTo);
                    break;
                case var method when method.Equals(JobType.DeleteSingle.ToString().ToLower()):
                    emailService.DeleteEmailsFromSender(serviceQuery.emailSenderToRemove);
                    break;
                case var method when method.Equals(JobType.DeleteAll.ToString().ToLower()):
                    emailService.DeleteEmailsFromSender(serviceQuery.emailSendersToRemove);
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