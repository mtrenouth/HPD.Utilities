using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities
{
    public interface IEmailService
    {
        void SendMessage(
                 string ToAddress,
                 string FromAddress,
                 string Subject,
                 string BodyHtml,
                 string BodyPlainText);

        Task SendMessageAsync(
                 string ToAddress,
                 string FromAddress,
                 string Subject,
                 string BodyHtml,
                 string BodyPlainText);
    }
}
