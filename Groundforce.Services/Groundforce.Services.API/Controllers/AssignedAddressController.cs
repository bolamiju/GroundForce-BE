using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Services.Core.Interfaces;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignedAddressController : ControllerBase
    {


        private AppDbContext _ctx;
        private readonly IMission _mission;

        public AssignedAddressController(AppDbContext ctx, IMission mission)
        {
            _ctx = ctx;
            _mission = mission;
        }





        [HttpGet("{userId}/ongoing/{page}")]
        public async Task<IActionResult> FetchAllOngoingMission(string userId, int page)
        {


            // page size 
            var per_page = 10;

            var allAddress = await _mission.FetchAllOngoingTask(userId);

            var missions = new List<MissionDTO>();

            foreach (var address in allAddress)
            {
                var Ongoingmission = new MissionDTO
                {
                    Remarks = address.Remarks,
                    LandMark = address.Landmark,
                    BusStop = address.BusStop,
                    AddressName = address.Address.AddressName,
                    BuildColor = address.BuildingColor
                };

                missions.Add(Ongoingmission);
            }

            // total pages 
            var totalPages = (int)Math.Ceiling(decimal.Divide(missions.Count(), per_page));

            // check if greater than totalpages 
            page = page > totalPages ? totalPages : page;
            //check if less than  0
            page = page < 0 ? 1 : page;
            //  paginated datas
            var pageData = missions.Skip((page - 1) * per_page).Take(per_page);

            // new dto
            var pageMission = new MissionPaginatedDTO();
            pageMission.TotalPages = missions.Count().ToString();
            pageMission.Per_page = per_page.ToString();
            pageMission.Page = page.ToString();
            pageMission.Data = pageData;


            return Ok(pageMission);


        }

    }
}
