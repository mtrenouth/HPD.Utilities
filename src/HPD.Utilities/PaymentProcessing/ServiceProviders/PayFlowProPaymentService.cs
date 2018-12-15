using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayPal.Payments.Common;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Transactions;
using PayPal.Payments.Communication;
using System.Configuration;

namespace HPD.Utilities.PaymentProcessing.ServiceProviders
{
    public class PayFlowProPaymentService : ICreditCardPaymentService
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
        public PayFlowProPaymentService(IConfirmationNumberGenerator ConfirmationNumberGenerator)
        {
            _confirmationNumberGenerator = ConfirmationNumberGenerator;
            Host = ConfigurationManager.AppSettings["PayFlowPro.Host"];
            Port = int.Parse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["PayFlowPro.Port"]) ? "443" : ConfigurationManager.AppSettings["PayFlowPro.Port"]);
            Timeout = int.Parse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["PayFlowPro.Timeout"]) ? "30" : ConfigurationManager.AppSettings["PayFlowPro.Timeout"]);
            User = ConfigurationManager.AppSettings["PayFlowPro.User"];
            Vendor = ConfigurationManager.AppSettings["PayFlowPro.Vendor"];
            Partner = ConfigurationManager.AppSettings["PayFlowPro.Partner"];
            Password = ConfigurationManager.AppSettings["PayFlowPro.Password"];
            Verbosity = ConfigurationManager.AppSettings["PayFlowPro.Verbosity"];
        }

        public PaymentResponse CreditCardPayment(CreditCardPaymentRequest req)
        {
            var resp = new PaymentResponse();
            var RequestID = PayflowUtility.RequestId;
            PayflowConnectionData Connection = new PayflowConnectionData(Host, Port <= 0 ? 443 : Port, Timeout <= 30 ? 30 : Timeout, "", 0, "", "");
            UserInfo PaymentProcessorUser = new UserInfo(User, Vendor, Partner, Password);
            int trxCount = 1;
            bool RespRecd = false;


            if (string.IsNullOrEmpty(req.InvoiceNumber))
                req.InvoiceNumber = _confirmationNumberGenerator.GenerateConfirmationNumber();


            Currency Amt = new Currency(req.Amount)
            {
                NoOfDecimalDigits = 0
            };
            Invoice Inv = new Invoice
            {
                Amt = Amt,
                Comment1 = req.Comment
            };

            //Set the BillTo object into invoice.
            if (req.BillingInformation != null)
                Inv.BillTo = CreateBillTo(req);

            CreditCard CC = new CreditCard(req.AccountNumber, req.Expiration.Replace("/", ""));
            CC.Cvv2 = req.CVC;

            // Create Card Tender data object.
            CardTender Card = new CardTender(CC);  // credit card

            // Notice we set the request id earlier in the application and outside our loop.  This way if a response was not received
            // but PayPal processed the original request, you'll receive the original response with DUPLICATE set.
            SaleTransaction Trans = new SaleTransaction(PaymentProcessorUser, Connection, Inv, Card, RequestID);
            Trans.Verbosity = String.IsNullOrEmpty(Verbosity) ? "HIGH" : Verbosity;
            while (trxCount <= 3 && !RespRecd)
            {

                // Submit the Transaction
                Response Resp = Trans.SubmitTransaction();

                // Display the transaction response parameters.
                if (Resp != null)
                {
                    RespRecd = true;  // Got a response.

                    // Get the Transaction Response parameters.
                    TransactionResponse TrxnResponse = Resp.TransactionResponse;

                    if (TrxnResponse != null)
                        resp = ProcessResponse(TrxnResponse);
                }
                else
                {
                    trxCount++;
                }
            }

            if (!RespRecd)
                resp.Message = "Payment not processed.  Please contact Customer Service";

            return resp;
        }

        private static PaymentResponse ProcessResponse(TransactionResponse TrxnResponse)
        {
            PaymentResponse resp = new PaymentResponse();
            decimal retAmt;
            resp.Transaction = TrxnResponse.Pnref;
            resp.AuthCode = TrxnResponse.AuthCode;

            if (decimal.TryParse(TrxnResponse.Amt, out retAmt))
            {
                resp.Amount = retAmt;
            }
            else
            {
                resp.Amount = 0;
            }

            resp.NumberShort = TrxnResponse.Acct;
            resp.ResponseCode = TrxnResponse.Result.ToString();
            resp.ResponseText = TrxnResponse.RespText;
            if (TrxnResponse.Result == 0) resp.Success = true;
            resp.Message = GetResonseMessage(TrxnResponse);
            return resp;
        }

        private static string GetResonseMessage(TransactionResponse TrxnResponse)
        {
            string RespMsg;
            if (TrxnResponse.Result < 0)
            {
                // Transaction failed.
                RespMsg = "There was an error processing your transaction." +
                    Environment.NewLine + "Error: " + TrxnResponse.Result.ToString();
            }
            else if (TrxnResponse.Result == 1 || TrxnResponse.Result == 26)
            {
                RespMsg = "Account configuration issue.  Please verify your login credentials.";
            }
            else if (TrxnResponse.Result == 0)
            {
                // Example of a message you might want to display with an approved transaction.
                RespMsg = "Your transaction was approved.";

            }
            else if (TrxnResponse.Result == 12)
            {
                // Hard decline from bank.  Customer will need to use another card or payment type.
                RespMsg = "Your transaction was declined.";
            }
            else if (TrxnResponse.Result == 13)
            {
                // Voice authorization required.  You would need to contact your merchant bank to obtain a voice authorization.  If authorization is 
                // given, you can manually enter it via Virtual Terminal in PayPal Manager or via the VoiceAuthTransaction object.
                RespMsg = "Your Transaction is pending. Contact Customer Service to complete your order.";
            }
            else if (TrxnResponse.Result == 23 || TrxnResponse.Result == 24)
            {
                // Issue with credit card number or expiration date.
                RespMsg = "Invalid credit card information. Please re-enter.";
            }
            else if (TrxnResponse.Result == 125)
            {
                RespMsg = "Your Transactions has been declined.";
            }
            else if (TrxnResponse.Result == 126)
            {
                // Decline transaction if AVS fails.
                if (TrxnResponse.AVSAddr != "Y" || TrxnResponse.AVSZip != "Y")
                {
                    RespMsg = "Your billing information does not match.  Please re-enter.";
                }
                else
                {
                    RespMsg = "Your Transaction is Under Review. We will notify you via e-mail if accepted.";
                }
                RespMsg = "Your Transaction is Under Review. We will notify you via e-mail if accepted.";
            }
            else if (TrxnResponse.Result == 127)
            {
                // There is an issue with checking this transaction through the fraud service.
                // You will need to manually approve.
                RespMsg = "Your Transaction is Under Review. We will notify you via e-mail if accepted.";
            }
            else
            {
                // Error occurred, display normalized message returned.
                RespMsg = TrxnResponse.RespMsg;
            }

            return RespMsg;
        }

        private static BillTo CreateBillTo(CreditCardPaymentRequest req)
        {
            // Create the BillTo object.
            BillTo Bill = new BillTo();
            Bill.BillToStreet = req.BillingInformation.BillingAddress;
            Bill.BillToStreet2 = req.BillingInformation.BillingAddressLine2;
            Bill.BillToCity = req.BillingInformation.BillingCity;
            Bill.BillToState = req.BillingInformation.BillingState;
            Bill.BillToZip = req.BillingInformation.BilingZipcode;
            return Bill;
        }

    }
}
