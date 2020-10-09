using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ResetUserPwdDTO
    {
        public string UserId { get; set; }
        public string CurrentPwd { get; set; }
        public string NewPwd { get; set; }
    }
}
