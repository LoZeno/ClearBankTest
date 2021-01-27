using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentServicePaymentSchemesTests
{
    public class FasterPaymentSchemeTest
    {
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;
        private const string ExistingDebtorAccountNumber = "debtorAccount";

        public FasterPaymentSchemeTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }
        
        [Fact]
        public void WhenRequestIsValid_AndAccountIsNotAllowedForFasterPayments_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };
            _mockDataStore.Setup(dataStore => dataStore.TryGetAccount(It.IsAny<string>(), out storedAccount)).Returns(true);
            
            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.FasterPayments);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"{PaymentScheme.FasterPayments} is not allowed for this account", paymentResult.ErrorMessage);
        }
        
        [Fact]
        public void WhenRequestIsValid_AndAccountIsAllowedForFasterPayments_ButBalanceIsLessThanPayment_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account
            {
                Balance = 90m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            };
            _mockDataStore.Setup(dataStore => dataStore.TryGetAccount(It.IsAny<string>(), out storedAccount)).Returns(true);
            
            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.FasterPayments);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"Insufficient Funds in account {ExistingDebtorAccountNumber}", paymentResult.ErrorMessage);
        }
    }
}