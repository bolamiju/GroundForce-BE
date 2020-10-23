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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class AssignedAddressController : ControllerBase
    {
        private AppDbContext _ctx;
        private readonly IMission _mission;
        private readonly IAddressRepo _addressRepo;
        private readonly ILogger<AssignedAddressController> _logger;

        public AssignedAddressController(AppDbContext ctx, IMission mission, IAddressRepo addressRepo, ILogger<AssignedAddressController> logger)
        {
            _ctx = ctx;
            _mission = mission;
            _addressRepo = addressRepo;
            _logger = logger;
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


        [Authorize(Roles = "Agent, Admin")]
        [HttpPatch("{id}/accepted")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] AssignedAddressUpdateDTO columnUpdate)
        {
            if (id < 1) return BadRequest("invalid parameter name");

            if (ModelState.IsValid)
            {
                try
                {
                    var updateAddress = await _addressRepo.UpdateAcceptedStatus(id, columnUpdate.Accepted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest("Could not update the address");
                }

                return Ok("Status Updated");
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Authorize(Roles = "Agent")]
        [Route("{userId:int}/reports/{pageNumber:int}")]
        public IActionResult FetchMission(int userId, int pageNumber)
        {
            FetchPaginatedMissionDTO missionsToReturn;
            List<AssignedAddresses> missions;
            try
            {
                missions = _mission.FetchMissions(userId);
                if (missions == null)
                {
                    return BadRequest("Agent does not exist");
                }

                pageNumber = pageNumber > 0 ? pageNumber : 1;
                var itemsPerPage = 10;
                var totalMissions = missions.Count;
                var totalPages = totalMissions % itemsPerPage == 0
                                            ? totalMissions / itemsPerPage
                                            : totalMissions / itemsPerPage + 1;

                var numberOfMissionsToSkip = (pageNumber - 1) * itemsPerPage;

                //assigns properties for the return value
                missionsToReturn = new FetchPaginatedMissionDTO
                {
                    Page = pageNumber,
                    PerPage = itemsPerPage,
                    Total = totalMissions,
                    TotalPages = totalPages,
                    AssignedAddresses = missions
                                        .Skip(numberOfMissionsToSkip)
                                        .Take(itemsPerPage).ToList()
                };
            }
            catch (Exception)
            {
                return BadRequest("Unable to fetch data");
            }
            return Ok(missionsToReturn);
        }
    }
}