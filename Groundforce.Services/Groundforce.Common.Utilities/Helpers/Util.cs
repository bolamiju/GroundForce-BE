﻿using Groundforce.Services.Models;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class Util
    {
        public static PageMetaData Paginate(int page, int per_page, int total)
        {
            int total_page = total % per_page == 0 ? total / per_page : total / per_page + 1;
            var result = new PageMetaData()
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