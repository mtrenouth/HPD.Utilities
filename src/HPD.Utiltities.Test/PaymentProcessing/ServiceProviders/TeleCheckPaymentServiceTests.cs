﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class TeleCheckPaymentServiceTests
    {
        HPD.Utilities.PaymentProcessing.IPayentService provider;
        [TestInitialize]
        public void Setup()
        {
            var ConfirmGenerator = new Mock<HPD.Utilities.PaymentProcessing.IConfirmationNumberGenerator>();
            ConfirmGenerator.Setup(x => x.GenerateConfirmationNumber()).Returns("MyConfirmation");
            provider = new HPD.Utilities.PaymentProcessing.ServiceProviders.TeleCheckPaymentService(ConfirmGenerator.Object)
            {
                Host = "pilot-PayflowPro.Paypal.com",
            };

        }
        

        [TestMethod()]
        public void BusinessCheckPaymentTest_Sucess()
        {
            var request = new BusinessCheckPaymentRequest
            {
                Amount = 10,
                BillingInformation = new BillingInformation
                {
                    BilingZipcode = "12345",
                    BillingAddress = "1234 Address Way",
                    BillingCity = "HomeTown",
                    BillingState = "CA",
                    BillToFirstName = "John",
                    BillToLastName = "Smith",
                    Country = "USA",
                    BillToEmail = "oneGreateUser@test.com",
                    BillToPhone = "9123214928"
                },
                RoutingNumber = "123456780",
                AccountNumber = "439085001",
                CheckNumber = "1001",
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
                EIN_SSN = "123345678",
                InvoiceNumber = "Inv12433"
            };
            var response = provider.BusinessCheckPayment(request);
            Assert.AreEqual(request.Amount, response.Amount);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.IsTrue(response.Success);

        }
        [TestMethod()]
        public void BusinessCheckPaymentTest_DeclinedNegativeData()
        {
            var request = new BusinessCheckPaymentRequest
            {
                Amount = 10,
                BillingInformation = new BillingInformation
                {
                    BilingZipcode = "12345",
                    BillingAddress = "1234 Address Way",
                    BillingCity = "HomeTown",
                    BillingState = "CA",
                    BillToFirstName = "John",
                    BillToLastName = "Smith",
                    Country = "USA",
                    BillToEmail = "oneGreateUser@test.com",
                    BillToPhone = "9123214928"
                },
                RoutingNumber = "11111",
                AccountNumber = "11111",
                CheckNumber = "1001",
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
                EIN_SSN = "123345678",
                InvoiceNumber = "Inv12433"
            };
            var response = provider.BusinessCheckPayment(request);
            Assert.AreEqual(request.Amount, response.Amount);
            Assert.AreEqual("12", response.ResponseCode);
            Assert.IsFalse(response.Success);
        }
        [TestMethod()]
        public void BusinessCheckPaymentTest_DeclinedRisk()
        {
            var request = new BusinessCheckPaymentRequest
            {
                Amount = 10,
                BillingInformation = new BillingInformation
                {
                    BilingZipcode = "12345",
                    BillingAddress = "1234 Address Way",
                    BillingCity = "HomeTown",
                    BillingState = "CA",
                    BillToFirstName = "John",
                    BillToLastName = "Smith",
                    Country = "USA",
                    BillToEmail = "oneGreateUser@test.com",
                    BillToPhone = "9123214928"
                },
                RoutingNumber = "222222",
                AccountNumber = "2222",
                CheckNumber = "1001",
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
                EIN_SSN = "123345678",
                InvoiceNumber = "Inv12433"
            };
            var response = provider.BusinessCheckPayment(request);
            Assert.AreEqual(request.Amount, response.Amount);
            Assert.AreEqual("12", response.ResponseCode);
            Assert.IsFalse(response.Success);
        }
        [TestMethod()]
        public void CheckPaymentTest()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                provider.CheckPayment(new CheckPaymentRequest
                {
                    Amount = 10,
                    BillingInformation = null,
                    RoutingNumber = "123456780",
                    AccountNumber = "439085001",
                    CheckNumber = "1001",
                    Comment = "Test Transacction",
                    IPAddress = "127.0.0.1",
                    InvoiceNumber = "Inv12433"
                });
            });
        }

        [TestMethod()]
        public void CreditCardPaymentTest()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                provider.CreditCardPayment(new CreditCardPaymentRequest
                {
                    Amount = 10,
                    AccountNumber = "4111111111111111",
                    Expiration = DateTime.Today.AddYears(1).ToString("MM/YYYY"),
                    Comment = "Test Transacction",
                    IPAddress = "127.0.0.1",
                });
            });
        }

        [TestMethod()]
        public void PersonalCheckPaymentTest_DeclinedNegativeData()
        {
            var request = new PersonalCheckPaymentRequest
            {
                Amount = 10,
                BillingInformation = new BillingInformation
                {
                    BilingZipcode = "12345",
                    BillingAddress = "1234 Address Way",
                    BillingCity = "HomeTown",
                    BillingState = "CA",
                    BillToFirstName = "John",
                    BillToLastName = "Smith",
                    Country = "USA",
                    BillToEmail = "oneGreateUser@test.com",
                    BillToPhone = "9123214928"
                },
                RoutingNumber = "11111",
                AccountNumber = "11111",
                CheckNumber = "1001",
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
                DriverLicenseNumber = "123345678",
                DriverLicenseState = "CA",
                InvoiceNumber = "Inv12433"
            };
            var response = provider.PersonalCheckPayment(request);
            Assert.AreEqual(request.Amount, response.Amount);
            Assert.AreEqual("12", response.ResponseCode);
            Assert.IsFalse(response.Success);
        }
        [TestMethod()]
        public void PersonalCheckPaymentTest_Declined()
        {
            var request = new PersonalCheckPaymentRequest
            {
                Amount = 10,
                BillingInformation = new BillingInformation
                {
                    BilingZipcode = "12345",
                    BillingAddress = "1234 Address Way",
                    BillingCity = "HomeTown",
                    BillingState = "CA",
                    BillToFirstName = "John",
                    BillToLastName = "Smith",
                    Country = "USA",
                    BillToEmail = "oneGreateUser@test.com",
                    BillToPhone = "9123214928"
                },
                RoutingNumber = "222222",
                AccountNumber = "2222",
                CheckNumber = "1001",
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
                DriverLicenseNumber = "123345678",
                DriverLicenseState = "CA",
                InvoiceNumber = "Inv12433"
            };
            var response = provider.PersonalCheckPayment(request);
            Assert.AreEqual(request.Amount, response.Amount);
            Assert.AreEqual("12", response.ResponseCode);
            Assert.IsFalse(response.Success);
        }
        [TestMethod()]
        public void PersonalCheckPaymentTest_Success()
        {
            var request = new PersonalCheckPaymentRequest
            {
                Amount = 10,
                BillingInformation = new BillingInformation
                {
                    BilingZipcode = "12345",
                    BillingAddress = "1234 Address Way",
                    BillingCity = "HomeTown",
                    BillingState = "CA",
                    BillToFirstName = "John",
                    BillToLastName = "Smith",
                    Country = "USA",
                    BillToEmail = "oneGreateUser@test.com",
                    BillToPhone = "9123214928"
                },
                RoutingNumber = "123456780",
                AccountNumber = "439085001",
                CheckNumber = "1001",
                Comment = "Test Transacction",
                IPAddress = "127.0.0.1",
                DriverLicenseNumber = "123345678",
                DriverLicenseState = "CA",
                InvoiceNumber = "Inv12433",
            };
            var response = provider.PersonalCheckPayment(request);
            Assert.AreEqual(request.Amount, response.Amount);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.IsTrue(response.Success);
        }
    }
}