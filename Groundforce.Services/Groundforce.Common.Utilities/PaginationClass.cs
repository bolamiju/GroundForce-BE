using CloudinaryDotNet.Actions;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Groundforce.Common.Utilities
{
     public class PaginationClass
    {


       public PaginationDTO Paginate(int page , int per_page , int total_page)
        {

            var result = new PaginationDTO()
            {
                TotalPages = total_page,
                PerPage = per_page,
                Page = page

            };
            return result;
        }
    }
}
