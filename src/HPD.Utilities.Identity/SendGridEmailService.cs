using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
namespace HPD.Utilities.Identity
{
    public class IdentityMessageEmailService : Microsoft.AspNet.Identity.IIdentityMessageService
    {
        HPD.Utilities.IEmailService emailService { get; set; }
        public string DefaultFromAddress { get; set; }

        public IdentityMessageEmailService(HPD.Utilities.IEmailService emailService)
        {
            this.emailService = emailService;
            var d = System.Configuration.ConfigurationManager.AppSettings["IdentityEmailFrom"];
            if (string.IsNullOrEmpty(d)) throw new ApplicationException("Missing Identtity Email From Configuration");
            DefaultFromAddress = d;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            await emailService.SendMessageAsync(message.Destination, DefaultFromAddress,
                message.Subject, message.Body,message.Body);
        }
    }
}
