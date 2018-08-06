using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.Communication;
using PayPal.Payments.DataObjects;

namespace HPD.Utilities.PaymentProcessing.ServiceProviders
{

    public class PayflowNetAPICreditCardPaymentService : ICreditCardPaymentService
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int Timeout { get; set; }
        public string User { get; set; }
        public string Vendor { get; set; }
        public string Partner { get; set; }
        public string Password { get; set; }
        public string Verbosity { get; set; }
        IConfirmationNumberGenerator _confirmationNumberGenerator;
        public PayflowNetAPICreditCardPaymentService(IConfirmationNumberGenerator ConfirmationNumberGenerator)
        {
            _confirmationNumberGenerator = ConfirmationNumberGenerator;
            Host = ConfigurationManager.AppSettings["PayflowNetAPI.Host"];
            Port = int.Parse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["PayflowNetAPI.Port"]) ? "443" : ConfigurationManager.AppSettings["PayflowNetAPI.Port"]);
            Timeout = int.Parse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["PayflowNetAPI.Timeout"]) ? "30" : ConfigurationManager.AppSettings["PayflowNetAPI.Timeout"]);
            User = ConfigurationManager.AppSettings["PayflowNetAPI.User"];
            Vendor = ConfigurationManager.AppSettings["PayflowNetAPI.Vendor"];
            Partner = ConfigurationManager.AppSettings["PayflowNetAPI.Partner"];
            Password = ConfigurationManager.AppSettings["PayflowNetAPI.Password"];
            Verbosity = ConfigurationManager.AppSettings["PayflowNetAPI.Verbosity"];
        }


        public PaymentResponse CreditCardPayment(CreditCardPaymentRequest req)
        {
            StringBuilder request = new StringBuilder();
            request.Append($"TRXTYPE=S");
            request.Append($"&ACCT={req.AccountNumber}");
            request.Append($"&EXPDATE={req.Expiration.Replace("/","")}");
            request.Append($"&TENDER=C");
            request.Append($"&INVNUM={req.ConfirmationNumber}");
            request.Append($"&AMT={req.Amount:0.##}");
            request.Append($"&USER={User}");
            request.Append($"&VENDOR={Vendor}");
            request.Append($"&PARTNER={Partner}");
            request.Append($"&PWD={Password}");
            request.Append($"&COMMENT1={req.Comment}");
            request.Append("&VERBOSITY=HIGH");
            PayflowNETAPI PayflowNETAPI = new PayflowNETAPI(Host, Port, Timeout);

            string PayflowResponse = PayflowNETAPI.SubmitTransaction(request.ToString(), PayflowUtility.RequestId);
            var results = PayflowResponse.Split(new char[] { '&' }).Select(x => new { Name = x.Split('=')[0], Value = x.Split('=')[1] });
            var RESPMSG = results.Where(x => x.Name == "RESPMSG").DefaultIfEmpty(null).Single().Value;
            if (RESPMSG == "Approved")
            {
                return new PaymentResponse
                {
                    Amount = req.Amount,
                    AuthCode = results.Where(x => x.Name == "AUTHCODE").DefaultIfEmpty(null).Single().Value,
                    Transaction = results.Where(x => x.Name == "PNREF").DefaultIfEmpty(null).Single().Value,
                    Success = true,
                    Message = RESPMSG
                };
            }
            else
            {
                return new PaymentResponse
                {
                    Amount = req.Amount,
                    Message = RESPMSG,
                    ResponseText = RESPMSG,
                    Success = false,
                };
            }

        }
    }
}
