using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.Models
{
    public class BankDetailsResponse
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string ResponseCode { get; set; }
        public string Message { get; set; }
    }
}
