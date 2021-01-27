using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSchemeValidators
{
    public class BacsSchemeValidator : IPaymentSchemeValidator
    {
        public (bool isValid, string validationMessage) IsRequestValidForPayment(
            MakePaymentRequest paymentRequest,
            Account debtorAccount)
        {
            if (!debtorAccount.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                return (false, $"{PaymentScheme.Bacs} is not allowed for this account");
            return (true, null);
        }
    }
}