using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class ResponseMessage
    {
        public static object Message(string errMsg)
        {
            return new { Message = errMsg };
        }
    }
}
