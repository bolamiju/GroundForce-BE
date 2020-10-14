using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Common.Utilities
{
    /// <summary>
    /// Status of phonenumber when checked in the Request table
    /// </summary>
    public enum PhoneNumberStatus
    {
        Verified,
        Blocked,
        ProcessRequest
    }
}
