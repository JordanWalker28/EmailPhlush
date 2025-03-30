using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Console = System.Console;

namespace EmailPhlush.Infrastructure
{
    public class EmailService(string imapServer, int port) : IEmailService, IDisposable
    {
        private readonly ImapClient _client = new();

        public void ConnectAndAuthenticate(string email, string password)
        {
            _client.Connect(imapServer, port, MailKit.Security.SecureSocketOptions.SslOnConnect);
            _client.Authenticate(email, password);
            Console.WriteLine("Connected and authenticated successfully.");
        }
        
        public void ScanEmails(DateTime startDate, DateTime endDate )
        {
            var senders = new Dictionary<string, int>();

            var allFolder = _client.GetFolder("[Gmail]/All Mail");
            allFolder.Open(FolderAccess.ReadWrite);
            
            var query = SearchQuery.DeliveredAfter(startDate).And(SearchQuery.DeliveredBefore(endDate));

            var uids = allFolder.Search(query);
            
            Console.WriteLine($"Found {uids.Count} emails between dates");

            foreach (var uid in uids)
            {
                var message = allFolder.GetMessage(uid);
                var senderAddress = message.From.Mailboxes.FirstOrDefault();

                if (senderAddress is null) continue;
                
                if (!senders.TryAdd(senderAddress.Address, 1))
                {
                    senders[senderAddress.Address]++;
                }
            }

            foreach (var sender in senders)
            {
                Console.WriteLine($"Found {sender.Key}, with {sender.Value}");
            }
        }

        public void DeleteEmailsFromSender(string senderEmail)
        {
            var allFolder = _client.GetFolder("[Gmail]/All Mail");
            allFolder.Open(FolderAccess.ReadWrite);
            var query = SearchQuery.FromContains(senderEmail);
            var uids = allFolder.Search(query);
            var trashFolder = _client.GetFolder(SpecialFolder.Trash);


            Console.WriteLine($"Total messages from {senderEmail}: {uids.Count}");
            
            if (uids.Count > 0)
            {
                Console.WriteLine($"Moving {uids.Count} emails from {senderEmail} to the Trash");
                allFolder.CopyTo(uids, trashFolder);
                Console.WriteLine($"Moved {uids.Count} messages to Trash.");
            }
            else
            {
                Console.WriteLine("No messages found from the specified sender.");
            }
            
            
            trashFolder.Open(FolderAccess.ReadWrite);
            var uidsInTrash = trashFolder.Search(query);
            Console.WriteLine($"Total messages from {senderEmail}: {uidsInTrash.Count}");
            trashFolder.Store (uidsInTrash, new StoreFlagsRequest (StoreAction.Add, MessageFlags.Deleted) { Silent = true });

            trashFolder.Expunge();
        }
        
        public void DeleteEmailsFromSender(List<string> senderEmails)
        {
            var allFolder = _client.GetFolder("[Gmail]/All Mail");
            allFolder.Open(FolderAccess.ReadWrite);

            foreach (var senderEmail in senderEmails)
            {
                allFolder = _client.GetFolder("[Gmail]/All Mail");
                allFolder.Open(FolderAccess.ReadWrite);
                
                var query = SearchQuery.FromContains(senderEmail);
                var uids = allFolder.Search(query);

                Console.WriteLine($"Total messages from {senderEmail}: {uids.Count}");
                
                if (uids.Count > 0)
                {
                    var trashFolder = _client.GetFolder(SpecialFolder.Trash);
                    
                    allFolder.CopyToAsync(uids, trashFolder);
                    
                    trashFolder.Expunge();

                    Console.WriteLine($"Moved {uids.Count} messages to Trash.");
                }
                else
                {
                    Console.WriteLine("No messages found from the specified sender.");
                }
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
            _client.Dispose();
        }
    }
}