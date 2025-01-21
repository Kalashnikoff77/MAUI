using System.Collections.Concurrent;

namespace SignalR.Models
{
    public class Accounts
    {
        public ConcurrentDictionary<string, AccountDetails> ConnectedAccounts { get; set; } = new();
    }

    public class AccountDetails
    {
        public int Id { get; set; }
        public string? Token { get; set; }
        public DateTime ConnectedDate { get; set; } = DateTime.Now;
    }
}
