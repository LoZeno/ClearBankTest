using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSchemeValidators
{
    internal class FasterPaymentsSchemeValidator : IPaymentSchemeValidator
    {
        public (bool isValid, string validationMessage) IsRequestValidForPayment(
            MakePaymentRequest paymentRequest,
            Account debtorAccount)
        {
            var (accountNumber, balance, _, allowedPaymentSchemes) = debtorAccount;
            if (!allowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                return (false, $"{PaymentScheme.FasterPayments} is not allowed for this account");

            if (balance < paymentRequest.Amount)
                return (false, $"Insufficient Funds in account {accountNumber}");

            return (true, string.Empty);
        }
    }
}