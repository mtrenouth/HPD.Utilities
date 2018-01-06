using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPD.Utilities.PaymentProcessing
{
    public class DefaultConfirmationNumberGenerator : IConfirmationNumberGenerator
    {
        public string GenerateConfirmationNumber()
        {
            int size = 10;
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString().ToLower();
        }
    }
}
