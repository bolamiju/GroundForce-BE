using CloudinaryDotNet.Actions;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Groundforce.Common.Utilities
{
     public static class PaginationUtil
    {


       public static PaginationDTO Paginate(int page , int per_page, int total)
        {
            int total_page = total % per_page == 0  ? total / per_page  : total / per_page + 1;
            var result = new PaginationDTO()
            {
                Page = page,
                PerPage = per_page,
                Total = total,
                TotalPages = total_page
            };
            return result;
        }
    }
}
