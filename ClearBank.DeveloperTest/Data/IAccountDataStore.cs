using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public interface IAccountDataStore
    {
        bool TryGetAccount(string accountNumber, out Account account);
        void UpdateAccount(Account account);
    }
}