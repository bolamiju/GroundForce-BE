using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UpdateQuestionOptionDTO
    {
        [Required]
        public string Option { get; set; }
    }
}
