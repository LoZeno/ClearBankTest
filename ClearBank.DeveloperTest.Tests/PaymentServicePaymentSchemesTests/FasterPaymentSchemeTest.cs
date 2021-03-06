﻿using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentServicePaymentSchemesTests
{
    public class FasterPaymentSchemeTest
    {
        private const string ExistingDebtorAccountNumber = "debtorAccount";
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly PaymentService _paymentService;

        public FasterPaymentSchemeTest()
        {
            _mockDataStore = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_mockDataStore.Object);
        }

        [Fact]
        public void WhenRequestIsValid_AndAccountIsNotAllowedForFasterPayments_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account(ExistingDebtorAccountNumber, 100m, AccountStatus.Live,
                AllowedPaymentSchemes.Bacs);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(It.IsAny<string>())).Returns((true, storedAccount));

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
        public void
            WhenRequestIsValid_AndAccountIsAllowedForFasterPayments_ButBalanceIsLessThanPayment_ReturnsFailedPayment_WithReason()
        {
            var storedAccount = new Account(ExistingDebtorAccountNumber, 90m, AccountStatus.Live,
                AllowedPaymentSchemes.FasterPayments);
            _mockDataStore.Setup(dataStore => dataStore.GetAccount(It.IsAny<string>())).Returns((true, storedAccount));

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