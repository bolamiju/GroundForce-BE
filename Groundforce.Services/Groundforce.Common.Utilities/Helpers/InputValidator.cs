using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class InputValidator
    {
        public static bool PhoneNumberValidator(string phoneNumber)
        {
            var regex = @"^\+\d{3}\d{9,10}$";
            var response = Regex.Match(phoneNumber, regex).Success;
            return response;
        }

        public static bool PinValidator(string pin)
        {
            var regex = @"^\d{4}$";
            var response = Regex.Match(pin, regex).Success;
            return response;
        }

        public static bool AccountNumberValidator(string accountNumber)
        {
            var regex = @"^\d{10}$";
            var response = Regex.Match(accountNumber, regex).Success;
            return response;
        }

        public static string WordInputValidator(Dictionary<string, string> regParams)
        {
            var result = "";

            foreach(var item in regParams)
            {
                if(!Regex.Match(item.Value, @"^\D+$").Success)
                {
                    result += item.Key + ", ";
                }
            }

            return result.Length > 0 ? result.Substring(0, result.Length - 2) : result;
        }
    }
}
