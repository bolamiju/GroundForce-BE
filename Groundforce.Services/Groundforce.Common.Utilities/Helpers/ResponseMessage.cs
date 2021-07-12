using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class ResponseMessage
    {
        public static object Message(string title, object errors = null, object data = null)
        {
            return new { title, errors, data };
        }
    }
}
