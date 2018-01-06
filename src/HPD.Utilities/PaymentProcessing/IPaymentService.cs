using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public interface IPayentService
    {
        PaymentResponse CreditCardPayment(CreditCardPaymentRequest req);
        PaymentResponse PersonalCheckPayment(PersonalCheckPaymentRequest req);
        PaymentResponse BusinessCheckPayment(BusinessCheckPaymentRequest req);
        PaymentResponse CheckPayment(CheckPaymentRequest req);
    }
}
