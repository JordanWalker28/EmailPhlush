using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace EmailPhlush
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("The application requires exactly 2 arguments.");
                return;
            }
            
            string imapServer = "imap.gmail.com";
            int port = 993;
            string email = args[0];
            string password =  args[1];

            using var client = new ImapClient();
            
            client.Connect(imapServer, port, MailKit.Security.SecureSocketOptions.SslOnConnect);

            client.Authenticate(email, password);
            
            client.Inbox.Open(FolderAccess.ReadWrite);

            var specificSender = "no-reply@email.game.co.uk";
            
            var query = SearchQuery.FromContains(specificSender);
            var uids = client.Inbox.Search(query);
            var trashFolder = client.GetFolder(SpecialFolder.Trash);

            Console.WriteLine($"Total messages from {specificSender}: {uids.Count}");

            foreach (var uid in uids)
            {
                client.Inbox.CopyTo(uid, trashFolder);
            }
            
            query = SearchQuery.FromContains(specificSender);
            uids = client.Inbox.Search(query);
            Console.WriteLine($"Total messages from {specificSender}: {uids.Count}");
            client.Inbox.Expunge();
            client.Disconnect(true);
        }
    }
}