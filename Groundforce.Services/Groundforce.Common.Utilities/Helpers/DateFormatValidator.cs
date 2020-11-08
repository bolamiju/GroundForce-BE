using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class DateFormatValidator
    {
        public static bool ValidateDate(string date)
        {
            bool dateValidity = DateTime.TryParseExact(
            date,
            "MM/dd/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);

            return dateValidity;
        }
    }
}
