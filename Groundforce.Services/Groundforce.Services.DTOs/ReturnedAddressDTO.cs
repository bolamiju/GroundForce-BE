using System;

namespace Groundforce.Services.DTOs
{
    public class ReturnedAddressDTO
    {
        public int AddressId { get; set; }
        public string AddressName { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}