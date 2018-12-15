using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public class CreditCardPaymentRequest
    {
        public BillingInformation BillingInformation { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string CVC { get; set; }
        public string Expiration { get; set; }
        public string InvoiceNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string IPAddress { get; set; }
        public string Comment { get; set; }
    }
}
