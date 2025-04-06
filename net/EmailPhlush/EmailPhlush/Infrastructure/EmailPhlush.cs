namespace EmailPhlush.Infrastructure;

public class EmailPhlush(IWriter writer, IEmailService emailService, IEmailConfig emailConfig)
    : IEmailPhlush
{
    
    public void Execute(ServiceQuery serviceQuery)
    {
        try
        {
            emailService.ConnectAndAuthenticate(emailConfig);

            switch (emailConfig.Method)
            {
                case JobType.Scan:
                    emailService.ScanEmails(serviceQuery.DateTimeFrom, serviceQuery.DateTimeTo);
                    break;
                case JobType.DeleteSingle:
                    emailService.DeleteEmailsFromSender(serviceQuery.EmailSenderToRemove);
                    break;
                case JobType.DeleteAll:
                    emailService.DeleteEmailsFromSender(serviceQuery.EmailSendersToRemove);
                    break;

                default:
                    writer.Write("Job Type Not Recognised");
                    break;
            }
        }
        catch (Exception ex)
        {
            writer.Write($"An error occurred: {ex.Message}");
        }
        finally
        {
            emailService.Disconnect();
        }
    }
}