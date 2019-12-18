using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOCVIDEO.PayPalWrapper
{
    public enum PaymentVerification_Flags : byte
    {
        INVALID = 1,
        OTHER = 2,
        VERIFIED = 0
    }
}
