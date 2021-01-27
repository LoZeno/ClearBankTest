using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSchemeValidators
{
    internal interface IPaymentSchemeValidator
    {
        (bool isValid, string validationMessage) IsRequestValidForPayment(MakePaymentRequest paymentRequest,
            Account debtorAccount);
    }
}