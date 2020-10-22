using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Common.Utilities;
using Groundforce.Services.Data;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
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
      [Authorize(Roles = "Agent")]
        public async Task<IActionResult> FetchAllOngoingMission(string userId, int page)
        {


            // page size 
           var per_page = 10;
          
            // instantiate address
            IEnumerable<AssignedAddresses> allAddress;
            try
            {
               allAddress = await _mission.FetchAllOngoingTask(userId, page, per_page);
            }
            catch (Exception)
            {

                return BadRequest("Invalid Detail");
            }

      
            //  first dto for address
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


            var paginationDetail = new PaginationClass();
           

            // new dto that contains pagination details 
            var pageMission = new MissionPaginatedDTO();
            pageMission.Pagination = paginationDetail.Paginate(page, per_page, _mission.TotalMissionAssigned);
            pageMission.Data = missions;

            return Ok(pageMission);


        }

    }
}
