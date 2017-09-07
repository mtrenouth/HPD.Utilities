using Microsoft.VisualStudio.TestTools.UnitTesting;
using HPD.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
namespace HPD.Utilities.Tests
{
    [TestClass()]
    public class SendGridEmailServiceTests
    {
        SendGrid.ISendGridClient client = null;
        [TestInitialize]
        public void Init()
        {
        }
        [TestMethod()]
        [TestCategory("Integration")]
        public async Task SendMessageTest()
        {
            SendGrid.Helpers.Mail.SendGridMessage message = null;
            var sendgridClient = new Mock<SendGrid.ISendGridClient>();
            sendgridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGrid.Helpers.Mail.SendGridMessage>(), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<SendGrid.Helpers.Mail.SendGridMessage, System.Threading.CancellationToken>((x, c) => message = x)
                .ReturnsAsync(It.IsAny<SendGrid.Response>());
            string ToAddress = "toAddress@test.com";
            string FromAddress = "fromAddress@test.com";
            string Subject = "Subject";
            string BodyHtml = "<p>Body</p>";
            string BodyPlainText = "Body";

            var sendgridService = new HPD.Utilities.SendGridEmailService(sendgridClient.Object);
            await sendgridService.SendMessage(ToAddress, FromAddress, Subject, BodyHtml, BodyPlainText);
            var emailPersonalization = message.Personalizations.First();
            Assert.AreEqual(message.From.Email, FromAddress);
            Assert.AreEqual(emailPersonalization.Subject, Subject);
            Assert.AreEqual(message.Contents.Where(x => x.Type == "text/html").First().Value, BodyHtml);
            Assert.AreEqual(message.Contents.Where(x => x.Type == "text/plain").First().Value, BodyPlainText);
            Assert.AreEqual(emailPersonalization.Tos.First().Email, ToAddress);

        }
    }
}