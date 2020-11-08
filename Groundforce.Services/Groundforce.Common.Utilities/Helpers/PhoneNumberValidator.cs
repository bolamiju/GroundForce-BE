using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class PhoneNumberValidator
    {
        public static bool PhoneNumberValid(string phoneNumber)
        {
            var regex = @"^\+\d{3}\d{9,10}$";
            var response = Regex.Match(phoneNumber, regex).Success;
            return response;
        }
    }
}
