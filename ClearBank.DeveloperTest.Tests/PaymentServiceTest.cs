using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceTest
    {
        private const string NonExistingDebtorAccountNumber = "nonExistingDebtorAccount";
        private const string ExistingDebtorAccountNumber = "debtorAccount";
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;

        public PaymentServiceTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }

        [Fact]
        public void WhenRequestIsValid_AndAccountDoesNotExist_ReturnsFailedPayment_WithReason()
        {
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(It.IsAny<string>()))
                .Returns((false, null));

            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                NonExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.FasterPayments);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);

            Assert.False(paymentResult.Success);
            Assert.Equal($"{NonExistingDebtorAccountNumber} is not a valid account number", paymentResult.ErrorMessage);
        }

        [Fact]
        public void WhenRequestIsValid_AndAccountExists_AndHasMatchingAllowedPaymentScheme_ReturnsSuccess()
        {
            var storedAccount = ExistingStoredAccount();
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(ExistingDebtorAccountNumber))
                .Returns((true, storedAccount));

            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.FasterPayments);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);

            Assert.True(paymentResult.Success);
        }

        [Theory]
        [InlineData(PaymentScheme.FasterPayments, AllowedPaymentSchemes.FasterPayments)]
        [InlineData(PaymentScheme.Bacs, AllowedPaymentSchemes.Bacs)]
        [InlineData(PaymentScheme.Chaps, AllowedPaymentSchemes.Chaps)]
        public void WhenRequestIsValid_AndAccountExists_AndHasMatchingAllowedPaymentScheme_UpdatesBalanceInDatastore(
            PaymentScheme requestedScheme, AllowedPaymentSchemes allowedScheme)
        {
            var storedAccount = ExistingStoredAccount(balance: 1000m, allowedScheme: allowedScheme);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(ExistingDebtorAccountNumber))
                .Returns((true, storedAccount));

            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                requestedScheme);

            _ = _paymentService.MakePayment(makePaymentRequest);

            _mockDataStore.Verify(datastore => datastore.UpdateAccount(
                It.Is<Account>(account => account.AccountNumber.Equals(ExistingDebtorAccountNumber)
                                          && account.Balance.Equals(900m)
                )));
        }

        private static Account ExistingStoredAccount(
            AllowedPaymentSchemes allowedScheme = AllowedPaymentSchemes.FasterPayments, decimal balance = 100m)
        {
            return new(ExistingDebtorAccountNumber, balance, AccountStatus.Live, allowedScheme);
        }
    }
}