using System;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Console = System.Console;

namespace EmailPhlush
{
    public class EmailService : IDisposable
    {
        private readonly string _imapServer;
        private readonly int _port;
        private readonly ImapClient _client;

        public EmailService(string imapServer, int port)
        {
            _imapServer = imapServer;
            _port = port;
            _client = new ImapClient();
        }

        public void ConnectAndAuthenticate(string email, string password)
        {
            _client.Connect(_imapServer, _port, MailKit.Security.SecureSocketOptions.SslOnConnect);
            _client.Authenticate(email, password);
            Console.WriteLine("Connected and authenticated successfully.");
        }

        public void DeleteEmailsFromSender(string senderEmail)
        {
            var allFolder = _client.GetFolder("[Gmail]/All Mail");
            allFolder.Open(FolderAccess.ReadWrite);
            var query = SearchQuery.FromContains(senderEmail);
            var uids = allFolder.Search(query);

            Console.WriteLine($"Total messages from {senderEmail}: {uids.Count}");

            if (uids.Count > 0)
            {
                var trashFolder = _client.GetFolder(SpecialFolder.Trash);
        
                foreach (var uid in uids)
                {
                    allFolder.CopyTo(uid, trashFolder);
                    allFolder.AddFlags(uid, MessageFlags.Deleted, true);
                }

                allFolder.Expunge();
                trashFolder.Open(FolderAccess.ReadWrite);
                trashFolder.Expunge();

                Console.WriteLine($"Moved {uids.Count} messages to Trash.");
            }
            else
            {
                Console.WriteLine("No messages found from the specified sender.");
            }
        }

        public void Disconnect()
        {
            if (!_client.IsConnected) return;
            _client.Disconnect(true);
            Console.WriteLine("Disconnected successfully.");
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}