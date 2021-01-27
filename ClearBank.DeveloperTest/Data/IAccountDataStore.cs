using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public interface IAccountDataStore
    {
        (bool exists, Account account) GetAccount(string accountNumber);
        void UpdateAccount(Account account);
    }
}