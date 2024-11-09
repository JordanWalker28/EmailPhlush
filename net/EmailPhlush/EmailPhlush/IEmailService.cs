namespace EmailPhlush;

public interface IEmailService
{
    void ConnectAndAuthenticate(string email, string password);
    void ScanEmails(DateTime from, DateTime to);
    void DeleteEmailsFromSender(string senderEmail);
    void DeleteEmailsFromSender(List<string> senderEmails);

    void Disconnect();
}