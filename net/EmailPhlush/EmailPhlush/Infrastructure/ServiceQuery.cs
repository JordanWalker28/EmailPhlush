namespace EmailPhlush.Infrastructure;

public class ServiceQuery
{
    public string EmailSenderToRemove = "comms@mailout.comms.premierleague.com";
    public DateTime DateTimeFrom = new(2025, 03, 30);
    public DateTime DateTimeTo =  DateTime.Now;
    public List<string> EmailSendersToRemove = ["contact@mailer.humblebundle.com"];
}