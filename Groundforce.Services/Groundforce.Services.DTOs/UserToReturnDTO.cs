using System;
using System.Collections.Generic;

namespace Groundforce.Services.DTOs
{
    public class UserToReturnDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Religion { get; set; }
        public string Email { get; set; }  
        public string AdditionalPhoneNumber { get; set; }
        public string ResidentialAddress { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AvatarUrl { get; set; }
        public string PublicId { get; set; }
    }

}
