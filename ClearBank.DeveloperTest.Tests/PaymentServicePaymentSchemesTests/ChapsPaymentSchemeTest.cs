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
        private const string ExistingDebtorAccountNumber = "debtorAccount";
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;

        public ChapsPaymentSchemeTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }

        [Fact]
        public void WhenRequestIsValid_AndAccountIsNotAllowedChaps_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account(ExistingDebtorAccountNumber, 100, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(It.IsAny<string>())).Returns((true, storedAccount));

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
        public void
            WhenRequestIsValid_AndAccountIsAllowedForChaps_AccountStatusIsNotLive_ReturnsFailedPayment_WithReason(
                AccountStatus accountStatus)
        {
            var storedAccount = new Account(ExistingDebtorAccountNumber, 100m, accountStatus, AllowedPaymentSchemes.Chaps);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(It.IsAny<string>())).Returns((true, storedAccount));

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