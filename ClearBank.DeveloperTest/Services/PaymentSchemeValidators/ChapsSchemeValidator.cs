using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSchemeValidators
{
    public class ChapsSchemeValidator : IPaymentSchemeValidator
    {
        public (bool isValid, string validationMessage) IsRequestValidForPayment(
            MakePaymentRequest paymentRequest,
            Account debtorAccount)
        {
            if (!debtorAccount.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                return (false, $"{PaymentScheme.Chaps} is not allowed for this account");

            if (debtorAccount.Status != AccountStatus.Live)
                return (false, $"{debtorAccount.AccountNumber} cannot perform payments");

            return (true, string.Empty);
        }
    }
}