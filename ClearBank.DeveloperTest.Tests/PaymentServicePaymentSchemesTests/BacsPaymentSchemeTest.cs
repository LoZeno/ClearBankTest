using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentServicePaymentSchemesTests
{
    public class BacsPaymentSchemeTest
    {
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;
        private const string ExistingDebtorAccountNumber = "debtorAccount";

        public BacsPaymentSchemeTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }
        
        [Fact]
        public void WhenRequestIsValid_AndAccountIsNotAllowedForBacs_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
            };
            _mockDataStore.Setup(dataStore => dataStore.TryGetAccount(It.IsAny<string>(), out storedAccount)).Returns(true);
            
            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                ExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.Bacs);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"{PaymentScheme.Bacs} is not allowed for this account", paymentResult.ErrorMessage);
        }
    }
}