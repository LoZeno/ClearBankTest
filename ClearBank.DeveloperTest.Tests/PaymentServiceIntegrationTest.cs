using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceIntegrationTest
    {
        private const string NonExistingDebtorAccountNumber = "nonExistingDebtorAccount";
        private const string ExistingDebtorAccountNumber = "debtorAccount";
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;

        public PaymentServiceIntegrationTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }

        [Theory]
        [InlineData(PaymentScheme.FasterPayments, AllowedPaymentSchemes.Bacs)]
        [InlineData(PaymentScheme.Bacs, AllowedPaymentSchemes.Chaps)]
        [InlineData(PaymentScheme.Chaps, AllowedPaymentSchemes.FasterPayments)]
        public void WhenRequestIsValid_AndAccountHasNoAllowedPaymentSchemes_ReturnsFailedPayment_WithReason(
            PaymentScheme requestedScheme, AllowedPaymentSchemes allowedScheme)
        {
            var storedAccount = new Account(ExistingDebtorAccountNumber, 100m, AccountStatus.Live, allowedScheme);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(It.IsAny<string>())).Returns((true, storedAccount));

            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                requestedScheme);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);

            Assert.False(paymentResult.Success);
            Assert.Equal($"{requestedScheme} is not allowed for this account", paymentResult.ErrorMessage);
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
            var storedAccount = new Account(ExistingDebtorAccountNumber, 1000m, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
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
        
        [Fact]
        public void WhenRequestIsValid_AndAccountExists_AndHasMatchingAllowedPaymentScheme_UpdatesBalanceInDatastore()
        {
            var storedAccount = new Account(ExistingDebtorAccountNumber, 1000m, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(ExistingDebtorAccountNumber))
                .Returns((true, storedAccount));

            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.FasterPayments);

            _ = _paymentService.MakePayment(makePaymentRequest);

            _mockDataStore.Verify(datastore => datastore.UpdateAccount(
                It.Is<Account>(account => account.AccountNumber.Equals(ExistingDebtorAccountNumber)
                                                && account.Balance.Equals(900m) 
                )));
        }
    }
}