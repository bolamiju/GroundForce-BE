using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Common.Utilities
{
    public enum PhoneNumberStatus
    {
        verified,
        blocked,
        invalidRequest,
        approved,
        pending,
        limitReached,
        confirmed
    }
}
