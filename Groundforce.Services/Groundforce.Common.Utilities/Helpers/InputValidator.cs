using System;
using System.Collections.Generic;
using System.Globalization;
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
      
        public static bool DateFormatValidator(string date)
        {
            bool dateValidity = DateTime.TryParseExact(
            date,
            "MM/dd/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);

            return dateValidity;
        }

        public static bool NUBANAccountValidator(string bankCode, string accountNumber)
        {
            bool result = false;
            try
            {
                if (bankCode.Trim().Length == 3 && accountNumber.Trim().Length == 10)
                {
                    var nuban = bankCode + accountNumber.Remove(9, 1);
                    var checkDigit = Convert.ToInt32(accountNumber.Substring(9, 1));
                    var calCheckDigit = CheckNUBANDigit(nuban);
                    calCheckDigit = calCheckDigit != 10 ? calCheckDigit : 0;
                    result = (checkDigit == calCheckDigit);
                }
            }
            catch { }
            return result;
        }

        private static int CheckNUBANDigit(string nuban)
        {
            var nubanArray = new int[nuban.Length];
            for (int i = 0; i < nuban.Length; i++)
            {
                nubanArray[i] = Convert.ToInt32(nuban[i].ToString());
            }
            var nubanSum = (nubanArray[0] * 3) + (nubanArray[1] * 7) + (nubanArray[2] * 3) + (nubanArray[3] * 3) +
                           (nubanArray[4] * 7) + (nubanArray[5] * 3) + (nubanArray[6] * 3) + (nubanArray[7] * 7) +
                           (nubanArray[8] * 3) + (nubanArray[9] * 3) + (nubanArray[10] * 7) + (nubanArray[11] * 3);
            return 10 - (nubanSum % 10);
        }
    }
}
