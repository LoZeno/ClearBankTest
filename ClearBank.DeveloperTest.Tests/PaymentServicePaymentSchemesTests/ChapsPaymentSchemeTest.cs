using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentServicePaymentSchemesTests
{
    public class ChapsPaymentSchemeTest
    {
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;
        private const string ExistingDebtorAccountNumber = "debtorAccount";

        public ChapsPaymentSchemeTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }
        
        [Fact]
        public void WhenRequestIsValid_AndAccountIsNotAllowedChaps_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            };
            _mockDataStore.Setup(dataStore => dataStore.TryGetAccount(It.IsAny<string>(), out storedAccount)).Returns(true);
            
            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.Chaps);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"{PaymentScheme.Chaps} is not allowed for this account", paymentResult.ErrorMessage);
        }
        
        [Theory]
        [InlineData(AccountStatus.Disabled)]
        [InlineData(AccountStatus.InboundPaymentsOnly)]
        public void WhenRequestIsValid_AndAccountIsAllowedForChaps_AccountStatusIsNotLive_ReturnsFailedPayment_WithReason(AccountStatus accountStatus)
        {
            var storedAccount = new Account
            {
                Status = accountStatus,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
            };
            _mockDataStore.Setup(dataStore => dataStore.TryGetAccount(It.IsAny<string>(), out storedAccount)).Returns(true);
            
            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.Chaps);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"{ExistingDebtorAccountNumber} cannot perform payments", paymentResult.ErrorMessage);
        }
    }
}