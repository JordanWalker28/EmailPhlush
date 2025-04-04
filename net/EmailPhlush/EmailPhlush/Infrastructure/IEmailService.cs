namespace EmailPhlush.Infrastructure;

public interface IEmailService
{
    void ConnectAndAuthenticate(IEmailConfig emailConfig);
    void ScanEmails(DateTime from, DateTime to);
    void DeleteEmailsFromSender(string senderEmail);
    void DeleteEmailsFromSender(List<string> senderEmails);
    void Disconnect();
}