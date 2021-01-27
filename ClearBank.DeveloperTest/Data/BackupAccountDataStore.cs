using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public class BackupAccountDataStore : IAccountDataStore
    {
        public (bool exists, Account account) GetAccount(string accountNumber)
        {
            // Access backup data base to retrieve account, code removed for brevity 
            return (true, new Account());
        }

        public void UpdateAccount(Account account)
        {
            // Update account in backup database, code removed for brevity
        }
    }
}