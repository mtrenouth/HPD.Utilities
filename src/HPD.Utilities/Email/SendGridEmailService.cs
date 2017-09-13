using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
namespace HPD.Utilities.Email
{
    public class SendGridEmailService : IEmailService
    {
        public static string DefaultFromAddress { get; set; }
        private ISendGridClient client { get; set; }

        public SendGridEmailService(ISendGridClient client)
        {
            this.client = client;
        }
        public void SendMessage(
                string ToAddress,
                string FromAddress,
                string Subject,
                string BodyHtml,
                string BodyPlainText = null
            )
        {
            var from = new EmailAddress(FromAddress);
            var subject = Subject;
            var to = new EmailAddress(ToAddress);
            var plainTextContent = BodyPlainText;
            var htmlContent = BodyHtml;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var result = client.SendEmailAsync(msg).Result;
        }
        public async Task SendMessageAsync(
                string ToAddress,
                string FromAddress,
                string Subject,
                string BodyHtml,
                string BodyPlainText = null
            )
        {
            var from = new EmailAddress(FromAddress);
            var subject = Subject;
            var to = new EmailAddress(ToAddress);
            var plainTextContent = BodyPlainText;
            var htmlContent = BodyHtml;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }

    }
}
