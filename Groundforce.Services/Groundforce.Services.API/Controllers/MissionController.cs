using Groundforce.Common.Utilities.Helpers;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes ="Bearer")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly IMissionRepository _missionRepository;
        private readonly ILogger<MissionController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAgentRepository _agentRepository;
        private int perPage = 10;

        public MissionController(IMissionRepository missionRepository, ILogger<MissionController> logger,
                                 UserManager<ApplicationUser> userManager, IAgentRepository agentRepository)
        {
            _missionRepository = missionRepository;
            _logger = logger;
            _userManager = userManager;
            _agentRepository = agentRepository;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AssignMission([FromBody] MissionTOAssignDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", ModelState));


            // generate a guid string as id for new entry
            var newId = "";
            Mission result = null;
            try
            {
                do
                {
                    newId = Guid.NewGuid().ToString();
                    result = await _missionRepository.GetMissionByIdForAgent(newId, model.FieldAgentId);
                } while (result != null);

                // get id of the currenly logged-in user
                var loggedInUserId = _userManager.GetUserId(User);

                // map the model values with a new object of mission
                var missionTOAssign = new Mission
                {
                    MissionId = newId,
                    VerificationItemId = model.VerificationItemId,
                    FieldAgentId = model.FieldAgentId,
                    AdminId = loggedInUserId
                };

                // Add to mission
                var newlyAssignedMission = await _missionRepository.AssignMission(missionTOAssign);
                if (!newlyAssignedMission)
                    return BadRequest(ResponseMessage.Message("Failed to assign mission"));

                return Ok(ResponseMessage.Message("Mission has been assigned sucessfully!"));

            }catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access failure, Please report to the technical team for support"));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> EditMissionAssigned(string id, [FromBody] MissionTOAssignDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", ModelState));

            if (String.IsNullOrWhiteSpace(id))
                return BadRequest(ResponseMessage.Message("Ensure Id is not empty of white-space"));

            try
            {
                // get mission by Id to edit
                var missionToEdit = await _missionRepository.GetMissionByIdForAgent(id, model.FieldAgentId);
                if (missionToEdit == null)
                    return NotFound(ResponseMessage.Message($"Could not find result with Id: {id}"));

                // get id of the currenly logged-in user
                var loggedInUserId = _userManager.GetUserId(User);

                // map data from model to the result fetched
                missionToEdit.FieldAgentId = model.FieldAgentId;
                missionToEdit.VerificationItemId = model.VerificationItemId;
                missionToEdit.AdminId = loggedInUserId;
                missionToEdit.UpdatedAt = DateTime.Now;

                // update the mission
                var result = await _missionRepository.EditMissionAssignment(missionToEdit);
                if(!result)
                    return NotFound(ResponseMessage.Message("Failed to edit mission"));

                return Ok(ResponseMessage.Message("Mission updated!"));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access failure, Please report to the technical team for support"));
            }
        }


        [Authorize(Roles = "Agent, Admin")]
        [HttpGet]
        [Route("{agentId}/{page}")]
        public async Task<IActionResult> GetMissionsForAgent(int page, string agentId)
        {
            if (String.IsNullOrWhiteSpace(agentId))
                return BadRequest(ResponseMessage.Message("Ensure Id is not empty or white-space"));

            // get logged in user
            var loggedInUser = await _userManager.GetUserAsync(User);

            try
            {
                // if logged in user is an agent, the logged in userId needs to mach with the agent id 
                // bug ---- the agent can be gotten either by agentId or User id
                // so when userid is provided it passes this check but returns no result cause, agent id is required to get mission
                var agent = await _agentRepository.GetAgentById(agentId);
                if(await _userManager.IsInRoleAsync(loggedInUser, "Agent"))
                {
                    if (agent.ApplicationUserId != loggedInUser.Id)
                        return Unauthorized(ResponseMessage.Message("Id provided does not match for logged-in agent"));
                }
                agentId = agent.FieldAgentId;


                // get missions assigned to user
                var results = await _missionRepository.GetAllMissionsForAgentPaginated(page, perPage, agentId);
                if (results == null || results.Count() < 1) // also testing for count of list returned
                    return NotFound(ResponseMessage.Message($"No result(s) found for page {page}"));

                // map missions fetched to return items dto
                var MissionsList = new List<ItemToReturnDTO>();
                foreach (var result in results)
                {
                    var MissionToReturn = new ItemToReturnDTO
                    {
                        ItemId = result.VerificationItem.ItemId,
                        ItemName = result.VerificationItem.ItemName,
                        AddedBy = null,
                        CreatedAt = result.UpdatedAt
                    };

                    MissionsList.Add(MissionToReturn);
                }

                //*************************************************************************************/
                //page = page > _missionRepository.TotalResultCount ? _missionRepository.TotalResultCount : page;
                page = page <= 0 ? 1 : page;
                //****************************************************************************************/

                // new dto that contains pagination details 
                var pagedItems = new PaginatedItemsToReturnDTO
                {
                    PageMetaData = Util.Paginate(page, perPage, _missionRepository.TotalResultCount),
                    Data = MissionsList
                };

                return Ok(ResponseMessage.Message("Items found", pagedItems));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Error fetching record(s)"));
            }
            
        }


        [Authorize(Roles = "Agent, Admin")]
        [HttpGet]
        [Route("ongoing/{agentId}/{page}")]
        public async Task<IActionResult> GetOngoingMissionsForAgent(int page, string agentId)
        {
            if (String.IsNullOrWhiteSpace(agentId))
                return BadRequest(ResponseMessage.Message("Bad request","Ensure Id is not empty or white-space"));

            // get logged in user
            var loggedInUser = await _userManager.GetUserAsync(User);

            try
            {
                // if logged in user is an agent, the logged in userId needs to mach with the agent id 
                if (await _userManager.IsInRoleAsync(loggedInUser, "Agent"))
                {
                    var agent = await _agentRepository.GetAgentById(agentId);
                    if (agent.ApplicationUserId != loggedInUser.Id)
                        return Unauthorized(ResponseMessage.Message("Id provided does not match for logged-in agent"));
                }

                // get missions assigned to user
                var results = await _missionRepository.GetAllOnGoingMissionsForAgentPaginated(page, perPage, agentId);
                if (results == null)
                    return NotFound(ResponseMessage.Message("Not found", "No result(s) found"));

                // map missions fetched to return items dto
                var MissionsList = new List<ItemToReturnDTO>();
                foreach (var result in results)
                {
                    var MissionToReturn = new ItemToReturnDTO
                    {
                        ItemId = result.VerificationItem.ItemId,
                        ItemName = result.VerificationItem.ItemName,
                        AddedBy = null,
                        CreatedAt = result.UpdatedAt
                    };

                    MissionsList.Add(MissionToReturn);
                }

                page = page <= 0 ? 1 : page;

                // new dto that contains pagination details 
                var pagedItems = new PaginatedItemsToReturnDTO
                {
                    PageMetaData = Util.Paginate(page, perPage, _missionRepository.TotalResultCount),
                    Data = MissionsList
                };

                return Ok(ResponseMessage.Message("Items found", pagedItems));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access failure, Please report to the technical team for support"));
            }

        }


        [Authorize(Roles = "Agent")]
        [HttpPatch]
        [Route("accept/{missionId}")]
        public async Task<IActionResult> AcceptMissionsForAgent(string missionId)
        {
            try
            {
                // get id of the loggedin user
                var loggedInUserId = _userManager.GetUserId(User);

                // get result by params
                var missionToUpdateStatus = await _missionRepository.GetMissionByIdForAgent(missionId, loggedInUserId);
                if (missionToUpdateStatus == null)
                    return NotFound(ResponseMessage.Message("No mission(s) found for agent"));

                // update result
                var result = await _missionRepository.UpdateMissionStatus(missionToUpdateStatus.MissionId, "accepted");
                if (!result)
                    return BadRequest(ResponseMessage.Message("Failed to accept mission"));

                return Ok(ResponseMessage.Message("Mission accepted!"));

            }catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access failure, Please report to the technical team for support"));
            }
        }


    }
}
