namespace EmailPhlush.Infrastructure;

public class ServiceQuery
{
    public string emailSenderToRemove = "news@linkedin.com";
    public DateTime dateTimeFrom = new(2025, 03, 30);
    public DateTime dateTimeTo =  DateTime.Now;
    public List<string> emailSendersToRemove = ["contact@mailer.humblebundle.com"];
}