using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ResetPinDto
    {
        public string Id { get; set; }
        public string CurrentPin { get; set; }
        public string NewPin { get; set; }
    }
}
