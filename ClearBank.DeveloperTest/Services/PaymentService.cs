using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore _accountDataStore;

        public PaymentService(IAccountDataStore accountDataStore)
        {
            _accountDataStore = accountDataStore;
        }
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            Account account = _accountDataStore.GetAccount(request.DebtorAccountNumber);

            // if (dataStoreType == "Backup")
            // {
            //     var accountDataStore = new BackupAccountDataStore();
            //     account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            // }
            // else
            // {
            //     var accountDataStore = new AccountDataStore();
            //     account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            // }

            var result = new MakePaymentResult();

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (account == null)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{request.DebtorAccountNumber} is not a valid account number";
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{PaymentScheme.Bacs} is not allowed for this account";
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (account == null)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{request.DebtorAccountNumber} is not a valid account number";
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{PaymentScheme.FasterPayments} is not allowed for this account";
                    }
                    else if (account.Balance < request.Amount)
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (account == null)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{request.DebtorAccountNumber} is not a valid account number";
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{PaymentScheme.Chaps} is not allowed for this account";
                    }
                    else if (account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
                    }
                    break;
            }

            if (result.Success)
            {
                account.Balance -= request.Amount;

                if (dataStoreType == "Backup")
                {
                    var accountDataStore = new BackupAccountDataStore();
                    accountDataStore.UpdateAccount(account);
                }
                else
                {
                    var accountDataStore = new AccountDataStore();
                    accountDataStore.UpdateAccount(account);
                }
            }

            return result;
        }
    }
}
