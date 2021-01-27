using System.Configuration;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceIntegrationTest
    {
        [Theory]
        [InlineData(PaymentScheme.FasterPayments)]
        [InlineData(PaymentScheme.Bacs)]
        [InlineData(PaymentScheme.Chaps)]
        public void WhenRequestIsValid_AndAccountHasNoAllowedPayments_ReturnsFailedPayment_WithReason(PaymentScheme requestedScheme)
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "NonBackup";

            var paymentService = new PaymentService();

            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = requestedScheme
            };
            
            var paymentResult = paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"{requestedScheme} is not allowed for this account", paymentResult.ErrorMessage);
        }
    }
}