using MailKit;
using MailKit.Net.Imap;

namespace EmailPhlush;

public class EmailFolderManager : IEmailFolderManager
{
    private readonly ImapClient _client;

    public EmailFolderManager(ImapClient client)
    {
        _client = client;
    }

    public IMailFolder GetFolder(string folderName)
    {
        return _client.GetFolder(folderName);
    }

    public IMailFolder GetSpecialFolder(SpecialFolder folder)
    {
        return _client.GetFolder(folder);
    }
}