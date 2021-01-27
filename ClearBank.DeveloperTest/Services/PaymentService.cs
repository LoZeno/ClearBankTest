using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.PaymentSchemeValidators;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore _accountDataStore;
        private readonly IDictionary<PaymentScheme, IPaymentSchemeValidator> _paymentSchemeValidators;

        public PaymentService(IAccountDataStore accountDataStore)
        {
            _accountDataStore = accountDataStore;
            _paymentSchemeValidators = InitializePaymentSchemeValidators();
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var (_, debtorAccountNumber, amount, _, paymentScheme) = request;

            var (accountExists, account) = _accountDataStore.GetAccount(debtorAccountNumber);
            if (!accountExists)
                return InvalidAccountResult(debtorAccountNumber);

            var paymentValidator = _paymentSchemeValidators[paymentScheme];

            var (isValid, validationMessage) = paymentValidator.IsRequestValidForPayment(request, account);

            if (!isValid)
                return AccountNotAllowedForPaymentResult(validationMessage);

            account.Balance -= amount;
            _accountDataStore.UpdateAccount(account);

            return PaymentSuccessfulResult();
        }

        private static MakePaymentResult PaymentSuccessfulResult()
        {
            return new() {Success = true};
        }

        private static MakePaymentResult AccountNotAllowedForPaymentResult(string validationMessage)
        {
            return new()
            {
                Success = false,
                ErrorMessage = validationMessage
            };
        }

        private static MakePaymentResult InvalidAccountResult(string debtorAccountNumber)
        {
            return new()
            {
                Success = false,
                ErrorMessage = $"{debtorAccountNumber} is not a valid account number"
            };
        }

        private static Dictionary<PaymentScheme, IPaymentSchemeValidator> InitializePaymentSchemeValidators()
        {
            return new()
            {
                {PaymentScheme.Bacs, new BacsSchemeValidator()},
                {PaymentScheme.Chaps, new ChapsSchemeValidator()},
                {PaymentScheme.FasterPayments, new FasterPaymentsSchemeValidator()}
            };
        }
    }
}