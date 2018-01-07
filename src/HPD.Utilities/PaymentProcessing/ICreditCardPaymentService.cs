using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public interface ICreditCardPaymentService
    {
        PaymentResponse CreditCardPayment(CreditCardPaymentRequest req);
    }
}
