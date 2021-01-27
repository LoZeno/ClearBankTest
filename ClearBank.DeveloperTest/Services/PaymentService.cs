using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

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
            var (_, debtorAccountNumber, amount, _, paymentScheme) = request;
            
            if (!_accountDataStore.TryGetAccount(debtorAccountNumber, out var account))
            {
                return InvalidAccountResult(debtorAccountNumber);
            }   

            var result = new MakePaymentResult{Success = true};

            switch (paymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{PaymentScheme.Bacs} is not allowed for this account";
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        result.Success = false;
                        result.ErrorMessage = $"{PaymentScheme.FasterPayments} is not allowed for this account";
                    }
                    else if (account.Balance < amount)
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
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
                account.Balance -= amount;
                _accountDataStore.UpdateAccount(account);
                
            }

            return result;
        }

        private static MakePaymentResult InvalidAccountResult(string debtorAccountNumber)
        {
            return new()
            {
                Success = false,
                ErrorMessage = $"{debtorAccountNumber} is not a valid account number"
            };
        }
    }
}
