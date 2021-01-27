using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSchemeValidators
{
    internal class ChapsSchemeValidator : IPaymentSchemeValidator
    {
        public (bool isValid, string validationMessage) IsRequestValidForPayment(
            MakePaymentRequest paymentRequest,
            Account debtorAccount)
        {
            var (accountNumber, _, accountStatus, allowedPaymentSchemes) = debtorAccount;
            if (!allowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                return (false, $"{PaymentScheme.Chaps} is not allowed for this account");

            if (accountStatus != AccountStatus.Live)
                return (false, $"{accountNumber} cannot perform payments");

            return (true, string.Empty);
        }
    }
}