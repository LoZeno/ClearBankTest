using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSchemeValidators
{
    public class FasterPaymentsSchemeValidator : IPaymentSchemeValidator
    {
        public (bool isValid, string validationMessage) IsRequestValidForPayment(
            MakePaymentRequest paymentRequest,
            Account debtorAccount)
        {
            if (!debtorAccount.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                return (false, $"{PaymentScheme.FasterPayments} is not allowed for this account");

            if (debtorAccount.Balance < paymentRequest.Amount)
                return (false, $"Insufficient Funds in account {debtorAccount.AccountNumber}");

            return (true, string.Empty);
        }
    }
}