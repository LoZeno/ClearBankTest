using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.PaymentSchemeValidators;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private static readonly IDictionary<PaymentScheme, IPaymentSchemeValidator> PaymentSchemeValidators =
            InitializePaymentSchemeValidators();

        private readonly IAccountDataStore _accountDataStore;

        public PaymentService(IAccountDataStore accountDataStore)
        {
            _accountDataStore = accountDataStore;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var (_, debtorAccountNumber, amount, _, paymentScheme) = request;

            var (accountExists, account) = _accountDataStore.GetAccount(debtorAccountNumber);
            if (!accountExists)
                return InvalidAccountResult(debtorAccountNumber);

            var paymentValidator = PaymentSchemeValidators[paymentScheme];
            var (isValid, validationMessage) = paymentValidator.IsRequestValidForPayment(request, account);
            if (!isValid)
                return AccountNotAllowedForPaymentResult(validationMessage);

            var newBalance = account.Balance - amount;
            _accountDataStore.UpdateAccount(account with { Balance = newBalance});

            return PaymentSuccessfulResult();
        }

        private static MakePaymentResult PaymentSuccessfulResult()
        {
            return new(true, null);
        }

        private static MakePaymentResult AccountNotAllowedForPaymentResult(string validationMessage)
        {
            return new(false, validationMessage);
        }

        private static MakePaymentResult InvalidAccountResult(string debtorAccountNumber)
        {
            return new(false, $"{debtorAccountNumber} is not a valid account number");
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