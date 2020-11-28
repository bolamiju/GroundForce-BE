using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class QuestionOptionDTO
    {
        [Required]
        public string Option { get; set; }

        [Display(Name = "Survey Question Id")]
        public string QuestionOptionId { get; set; }
    }
}
