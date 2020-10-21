using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Groundforce.Services.Data.Services
{
    public interface IAddressRepo
    {
        Task<bool> UpdateAddress(Address model);
    }
}