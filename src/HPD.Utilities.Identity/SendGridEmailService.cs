//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Identity;
//using SendGrid;
//using SendGrid.Helpers.Mail;
//using System.Configuration;
//namespace HPD.Utilities
//{
//    public class SendGridEmailService : Microsoft.AspNet.Identity.IIdentityMessageService
//    {
//        public SendGridEmailService()
//        {
//        }


//        public async Task SendMessage(
//                string ToAddress,
//                string FromAddress,
//                string Subject,
//                string BodyHtml,
//                string BodyPlainText = null
//            )
//        {
//            var from = new EmailAddress(FromAddress);
//            var subject = Subject;
//            var to = new EmailAddress(ToAddress);
//            var plainTextContent = BodyPlainText;
//            var htmlContent = BodyHtml;
//            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
//            await client.SendEmailAsync(msg);
//        }

//        public async Task SendAsync(IdentityMessage message)
//        {
//            await SendMessage(message.Destination, SendGridEmailService.DefaultFromAddress,
//                message.Subject, message.Body);
//        }
//    }
//}
