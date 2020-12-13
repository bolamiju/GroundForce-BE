using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class UserSurvey
    {
        [Display(Name = "User Id")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        [Display(Name = "Survey Id")]
        public string SurveyId { get; set; }
        public Survey Survey { get; set; }

        public string Status { get; set; } = "pending";
        public ICollection<Response> Responses { get; set; }
    }
}
