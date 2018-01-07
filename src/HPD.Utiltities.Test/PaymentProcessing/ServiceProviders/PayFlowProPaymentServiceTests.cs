using Microsoft.VisualStudio.TestTools.UnitTesting;
using HPD.Utilities.PaymentProcessing.ServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace HPD.Utilities.PaymentProcessing.ServiceProviders.Tests
{
    [TestClass()]
    [TestCategory("Integration")]
    [TestCategory("Paypal")]
    public class PayFlowProPaymentServiceTests
    {
        HPD.Utilities.PaymentProcessing.ICreditCardPaymentService provider;
        [TestInitialize]
        public void Setup()
        {
            var ConfirmGenerator = new Mock<HPD.Utilities.PaymentProcessing.IConfirmationNumberGenerator>();
            ConfirmGenerator.Setup(x => x.GenerateConfirmationNumber()).Returns("MyConfirmation");
            provider = new HPD.Utilities.PaymentProcessing.ServiceProviders.PayFlowProPaymentService(ConfirmGenerator.Object)
            {
                Host = "pilot-PayflowPro.Paypal.com",
            };

        }


        [TestMethod()]
        public void CreditCardPaymentTest_Succcess()
        {
            var Request = new CreditCardPaymentRequest
            {
                Amount = 10,
                AccountNumber = "4111111111111111",
                Expiration = DateTime.Today.AddYears(1).ToString("MM/YYYY"),
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
            };
            var response = provider.CreditCardPayment(Request);
            Assert.AreEqual(Request.Amount, response.Amount);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("0", response.ResponseCode);
        }
        [TestMethod()]
        public void CreditCardPaymentTest_Decline()
        {
            var Request = new CreditCardPaymentRequest
            {
                Amount = 10417,
                AccountNumber = "4111111111111111",
                Expiration = DateTime.Today.AddYears(1).ToString("MM/yyyy"),
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
                CVC = "123"
            };
            var response = provider.CreditCardPayment(Request);
            Assert.AreEqual(Request.Amount, response.Amount);
            Assert.IsFalse(response.Success);
            Assert.AreEqual("12", response.ResponseCode);
        }

    }
}