using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public class BackupAccountDataStore : IAccountDataStore
    {
        public bool TryGetAccount(string accountNumber, out Account account)
        {
            // Access backup data base to retrieve account, code removed for brevity 
            account = new Account();
            return true;
        }

        public void UpdateAccount(Account account)
        {
            // Update account in backup database, code removed for brevity
        }
    }
}
