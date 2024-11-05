using MailKit;

public interface IEmailFolderManager
{
    IMailFolder GetFolder(string folderName);
    IMailFolder GetSpecialFolder(SpecialFolder folder);
}