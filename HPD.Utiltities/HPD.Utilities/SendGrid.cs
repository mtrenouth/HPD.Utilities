using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace HPD.Utilities
{
    public class SendGridEmailService : Microsoft.AspNet.Identity.IIdentityMessageService
    {
        public  static string APIKey { get { return "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"; } }
        public static string DefaultFromAddress { get; set; }

        static async Task SendMessage(
                string ToAddress,
                string FromAddress,
                string Subject,
                string BodyHtml,
                string BodyPlainText = null
            )
        {
            var client = new SendGridClient(APIKey);
            var from = new EmailAddress(FromAddress);
            var subject = Subject;
            var to = new EmailAddress(ToAddress);
            var plainTextContent = BodyPlainText;
            var htmlContent = BodyHtml;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public Task SendAsync(IdentityMessage message)
        {
            return SendGridEmailService.SendMessage(message.Destination, SendGridEmailService.DefaultFromAddress,
                message.Subject, message.Body);
        }
    }
}
