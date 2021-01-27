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
        private readonly PaymentService _paymentService;
        private readonly Mock<IAccountDataStore> _mockDataStore;

        public PaymentServiceIntegrationTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }
        
        [Theory]
        [InlineData(PaymentScheme.FasterPayments, AllowedPaymentSchemes.Bacs)]
        [InlineData(PaymentScheme.Bacs, AllowedPaymentSchemes.Chaps)]
        [InlineData(PaymentScheme.Chaps, AllowedPaymentSchemes.FasterPayments)]
        public void WhenRequestIsValid_AndAccountHasNoAllowedPaymentSchemes_ReturnsFailedPayment_WithReason(PaymentScheme requestedScheme, AllowedPaymentSchemes allowedScheme)
        {
            var storedAccount = new Account
            {
                AllowedPaymentSchemes = allowedScheme
            };
            _mockDataStore.Setup(dataStore => dataStore.TryGetAccount(It.IsAny<string>(), out storedAccount)).Returns(true);
            
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
            Account account = null;
            _mockDataStore.Setup(dataStore =>dataStore.TryGetAccount(It.IsAny<string>(), out account))
                .Returns(false);
            
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
            var storedAccount = new Account
            {
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            };
            _mockDataStore.Setup(dataStore =>dataStore.TryGetAccount(ExistingDebtorAccountNumber, out storedAccount))
                .Returns(true);
            
            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.FasterPayments);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);
            
            Assert.True(paymentResult.Success);
        }
    }
}