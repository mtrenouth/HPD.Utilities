using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public class PersonalCheckPaymentRequest : CheckPaymentRequest
    {
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseState { get; set; }
    }
}
