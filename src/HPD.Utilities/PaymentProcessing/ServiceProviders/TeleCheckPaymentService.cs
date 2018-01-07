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
    public class TeleCheckPaymentService : IBusinessCheckPaymentService, IPersonalCheckPaymentService
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
        public TeleCheckPaymentService(IConfirmationNumberGenerator ConfirmationNumberGenerator)
        {
            _confirmationNumberGenerator = ConfirmationNumberGenerator;
            Host = ConfigurationManager.AppSettings["TeleCheck.Host"];
            Port = int.Parse(String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TeleCheck.Port"]) ? "443": ConfigurationManager.AppSettings["TeleCheck.Port"]);
            Timeout = int.Parse(String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TeleCheck.Timeout"]) ? "30" : ConfigurationManager.AppSettings["TeleCheck.Timeout"]);
            User = ConfigurationManager.AppSettings["TeleCheck.User"];
            Vendor = ConfigurationManager.AppSettings["TeleCheck.Vendor"];
            Partner = ConfigurationManager.AppSettings["TeleCheck.Partner"];
            Password = ConfigurationManager.AppSettings["TeleCheck.Password"];
            Verbosity = ConfigurationManager.AppSettings["TeleCheck.Verbosity"];
        }
        public PaymentResponse BusinessCheckPayment(BusinessCheckPaymentRequest req)
        {
            var resp = new PaymentResponse();
            Invoice Inv = new Invoice();
            var RequestID = PayflowUtility.RequestId;
            PayflowConnectionData Connection = new PayflowConnectionData(Host, Port, Timeout, "", 0, "", "");
            int trxCount = 1;
            bool RespRecd = false;

            Currency Amt = new Currency(req.Amount);
            Amt.NoOfDecimalDigits = 0;
            Inv.Amt = Amt;
            Inv.InvNum = req.InvoiceNumber;
            Inv.Comment1 = req.Comment;

            // Create the BillTo object.
            Inv.BillTo = CreateBillTo(req.BillingInformation);

            // Create Credit Card data object. 
            var payment = new PayPal.Payments.DataObjects.CheckPayment(req.RoutingNumber + req.AccountNumber + req.CheckNumber);

            // Create Card Tender data object.
            var tender = new CheckTender(payment)
            {
                ChkType = "C",
                SS = req.EIN_SSN,
                ChkNum = req.CheckNumber
            }; 


            UserInfo TeleCheckUser = new UserInfo(User, Vendor, Partner, Password);

            // Notice we set the request id earlier in the application and outside our loop.  This way if a response was not received
            // but PayPal processed the original request, you'll receive the original response with DUPLICATE set.
            AuthorizationTransaction Trans = new AuthorizationTransaction(TeleCheckUser, Connection, Inv, tender, RequestID);
            Trans.AddToExtendData(new ExtendData("AUTHTYPE", "I"));
            Trans.AddToExtendData(new ExtendData("CUSTIP", req.IPAddress));

            Trans.Verbosity = String.IsNullOrEmpty(Verbosity)? "HIGH" : Verbosity;


            while (trxCount <= 3 && !RespRecd)
            {

                Response Resp = Trans.SubmitTransaction();
                if (Resp != null)
                {
                    RespRecd = true;  // Got a response.
                    TransactionResponse TrxnResponse = Resp.TransactionResponse;
                    if (TrxnResponse != null)
                        resp = ProcessTransaction(TrxnResponse);
                }
                else
                {
                    trxCount++;
                }
            }

            if (!RespRecd)
            {
                resp.Success = false;
                resp.Message = "Payment not processed.  Please contact Customer Service";
            }

            return resp;
        }

        public PaymentResponse PersonalCheckPayment(PersonalCheckPaymentRequest req)
        {
            var resp = new PaymentResponse();
            Invoice Inv = new Invoice();
            var RequestID = PayflowUtility.RequestId;
            PayflowConnectionData Connection = new PayflowConnectionData(Host, Port, Timeout, "", 0, "", "");
            int trxCount = 1;
            bool RespRecd = false;

            Currency Amt = new Currency(req.Amount);
            Amt.NoOfDecimalDigits = 0;
            Inv.Amt = Amt;
            Inv.InvNum = req.InvoiceNumber;
            Inv.Comment1 = req.Comment;

            // Create the BillTo object.
            Inv.BillTo = CreateBillTo(req.BillingInformation);

            // Create Credit Card data object. 
            var payment = new PayPal.Payments.DataObjects.CheckPayment(req.RoutingNumber + req.AccountNumber + req.CheckNumber);

            // Create Check Tender data object.
            var tender = new CheckTender(payment)
            {
                ChkType = "P",
                DL = req.DriverLicenseState + req.DriverLicenseNumber,
                ChkNum = req.CheckNumber
            };


            UserInfo TeleCheckUser = new UserInfo(User, Vendor, Partner, Password);

            // Notice we set the request id earlier in the application and outside our loop.  This way if a response was not received
            // but PayPal processed the original request, you'll receive the original response with DUPLICATE set.
            AuthorizationTransaction Trans = new AuthorizationTransaction(TeleCheckUser, Connection, Inv, tender, RequestID);
            Trans.AddToExtendData(new ExtendData("AUTHTYPE", "I"));
            Trans.AddToExtendData(new ExtendData("CUSTIP", req.IPAddress));

            Trans.Verbosity = String.IsNullOrEmpty(Verbosity) ? "HIGH" : Verbosity;


            while (trxCount <= 3 && !RespRecd)
            {

                Response Resp = Trans.SubmitTransaction();
                if (Resp != null)
                {
                    RespRecd = true;  // Got a response.
                    TransactionResponse TrxnResponse = Resp.TransactionResponse;
                    if (TrxnResponse != null)
                        resp = ProcessTransaction(TrxnResponse);
                }
                else
                {
                    trxCount++;
                }
            }

            if (!RespRecd)
            {
                resp.Success = false;
                resp.Message = "Payment not processed.  Please contact Customer Service";
            }

            return resp;
        }

        private static PaymentResponse ProcessTransaction(TransactionResponse TrxnResponse)
        {
            PaymentResponse resp = new PaymentResponse();
            resp.Transaction = TrxnResponse.Pnref;
            resp.AuthCode = TrxnResponse.AuthCode;
            resp.Amount = decimal.Parse(TrxnResponse.Amt);
            resp.NumberShort = TrxnResponse.Acct;
            if (TrxnResponse.Result == 0) resp.Success = true;
            resp.ResponseCode = TrxnResponse.Result.ToString();
            resp.Message = CreateResponseMessage(TrxnResponse);
            return resp;
        }

        private static string CreateResponseMessage(TransactionResponse TrxnResponse)
        {
            string RespMsg;
            // Evaluate Result Code
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

        private static BillTo CreateBillTo(BillingInformation billingInformation)
        {
            if (billingInformation == null) return null;
            BillTo Bill = new BillTo();
            Bill.BillToStreet = billingInformation.BillingAddress;
            Bill.BillToStreet2 = billingInformation.BillingAddressLine2;
            Bill.BillToCity = billingInformation.BillingCity;
            Bill.BillToState = billingInformation.BillingState;
            Bill.BillToZip = billingInformation.BilingZipcode;
            Bill.BillToCountry = billingInformation.Country;
            Bill.BillToFirstName = billingInformation.BillToFirstName;
            Bill.BillToLastName = billingInformation.BillToLastName;
            Bill.BillToEmail = billingInformation.BillToEmail;
            Bill.BillToPhone = billingInformation.BillToPhone;
            return Bill;
        }

    }
}
