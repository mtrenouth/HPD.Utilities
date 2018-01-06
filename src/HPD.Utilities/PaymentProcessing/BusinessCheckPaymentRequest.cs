using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public class BusinessCheckPaymentRequest : CheckPaymentRequest
    {
        public string EIN_SSN { get; set; }

    }
}
