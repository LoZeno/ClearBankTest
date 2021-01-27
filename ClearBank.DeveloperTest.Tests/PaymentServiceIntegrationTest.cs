using System;
using System.Configuration;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceIntegrationTest
    {
        private const string NonExistingDebtorAccountNumber = "nonExistingDebtorAccount";
        private const string ExistingDebtorAccountNumber = "debtorAccount";
        private readonly PaymentService _paymentService;

        public PaymentServiceIntegrationTest()
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "NonBackup";
            _paymentService = new PaymentService();
        }
        
        [Theory]
        [InlineData(PaymentScheme.FasterPayments)]
        [InlineData(PaymentScheme.Bacs)]
        [InlineData(PaymentScheme.Chaps)]
        public void WhenRequestIsValid_AndAccountHasNoAllowedPaymentSchemes_ReturnsFailedPayment_WithReason(PaymentScheme requestedScheme)
        {
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
            var makePaymentRequest = new MakePaymentRequest(
                "creditorAccount",
                NonExistingDebtorAccountNumber,
                100.0m, DateTime.Now,
                PaymentScheme.FasterPayments);

            var paymentResult = _paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"{NonExistingDebtorAccountNumber} is not a valid account number", paymentResult.ErrorMessage);
        }
    }
}