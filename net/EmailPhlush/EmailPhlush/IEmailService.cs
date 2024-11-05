namespace EmailPhlush;

public interface IEmailService : IDisposable
{
    void ConnectAndAuthenticate(string email, string password);
    void ScanEmails(DateTime startDate, DateTime endDate);
    void DeleteEmailsFromSender(string senderEmail);
    void Disconnect();
}