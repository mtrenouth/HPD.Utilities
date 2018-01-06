using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public class BillingInformation
    {
        public string BillingAddress { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BilingZipcode { get; set; }
        public string Country { get; set; }
        public string BillToFirstName { get; set; }
        public string BillToEmail { get; set; }
        public string BillToLastName { get; set; }
        public string BillToPhone { get; set; }

    }
}
