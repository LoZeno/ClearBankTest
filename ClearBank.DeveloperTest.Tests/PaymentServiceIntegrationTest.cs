﻿using System.Configuration;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceIntegrationTest
    {
        [Fact]
        public void WhenRequestIsValid_AndAccountHasNoAllowedPayments_ReturnsFailedPayment_WithReason()
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "NonBackup";

            var paymentService = new PaymentService();

            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments
            };
            
            var paymentResult = paymentService.MakePayment(makePaymentRequest);
            
            Assert.False(paymentResult.Success);
            Assert.Equal($"{PaymentScheme.FasterPayments} is not allowed for this account", paymentResult.ErrorMessage);
        }
    }
}