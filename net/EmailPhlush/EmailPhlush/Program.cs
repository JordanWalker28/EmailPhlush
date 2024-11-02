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

            var specificSender = "no-reply@spotify.com";
        
            var query = SearchQuery.FromContains(specificSender);
            var uids = client.Inbox.Search(query);

            Console.WriteLine($"Total messages from {specificSender}: {uids.Count}");

            if (uids.Count > 0)
            {
                var firstMessageUid = uids[0];
                client.Inbox.AddFlags(firstMessageUid, MessageFlags.Deleted, true);
                Console.WriteLine("First message from specified sender marked for deletion.");

                client.Inbox.Expunge();
                Console.WriteLine("Deleted message permanently removed.");
            }
            else
            {
                Console.WriteLine("No messages found from the specified sender.");
            }

            query = SearchQuery.FromContains(specificSender);
            uids = client.Inbox.Search(query);
            Console.WriteLine($"Total messages from {specificSender}: {uids.Count}");

            client.Disconnect(true);
        }
    }
}