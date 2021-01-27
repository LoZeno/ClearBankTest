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
        private const string ExistingDebtorAccountNumber = "debtorAccount";
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;

        public BacsPaymentSchemeTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }

        [Fact]
        public void WhenRequestIsValid_AndAccountIsNotAllowedForBacs_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account(ExistingDebtorAccountNumber, 100, AccountStatus.Live, AllowedPaymentSchemes.Chaps);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(It.IsAny<string>())).Returns((true, storedAccount));

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