using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public class PaymentResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string NumberShort { get; set; }
        public string Transaction { get; set; }
        public string AuthCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Confirmation { get; set; }
        public string ProviderResponse { get; set; }
    }
}
