using Groundforce.Common.Utilities.Helpers;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly int perPage;

        public MissionController(IMissionRepository missionRepository, ILogger<MissionController> logger,
                                 UserManager<ApplicationUser> userManager, IAgentRepository agentRepository,
                                 IConfiguration configrutation)
        {
            _missionRepository = missionRepository;
            _logger = logger;
            _userManager = userManager;
            _agentRepository = agentRepository;
            perPage = Convert.ToInt32(configrutation.GetSection("PaginationSettings:PerPage").Get<string>());
        }


        [Authorize(Roles = "Admin, Client")]
        [HttpPost]
        [Route("add-address")]
        public async Task<IActionResult> AddAddress([FromBody] ItemToAddDTO model)
        {

            try
            {
                // ensure model state is valid
                if (!ModelState.IsValid)
                    return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

                // generate item id
                string itemId = "";
                VerificationItem result = null;
                do
                {
                    itemId = Guid.NewGuid().ToString();
                    result = await _missionRepository.GetVerificationItemById(itemId);
                } while (result != null);

                // construct item
                var newItem = new VerificationItem
                {
                    ItemId = itemId,
                    ApplicationUserId = _userManager.GetUserId(User),
                    Title = model.Title,
                    Description = model.Description
                };

                // add item
                if(!await _missionRepository.Add(newItem))
                    return BadRequest(ResponseMessage.Message("Failed to added", errors: "Could not add record to data source"));

                return Ok(ResponseMessage.Message("Added successfully", data: new { AddressId = newItem.ItemId }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
                        
        }


        [Authorize(Roles = "Admin, Client")]
        [HttpPut]
        [Route("edit-address")]
        public async Task<IActionResult> EditAddress([FromBody] ItemToEditDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

            // check if id is not null or empty
            if(String.IsNullOrWhiteSpace(model.Id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: "Id should not be null or empty or whitespace"));

            try
            {
                // get item using id
                var item = await _missionRepository.GetVerificationItemById(model.Id);
                if (item == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: $"Item with id: {model.Id} was not found"));

                // re-assign values
                item.Title = model.Title;
                item.Description = model.Description;


                // update data source
                var result = await _missionRepository.Update(item);
                if (!result)
                    return BadRequest(ResponseMessage.Message("Failed to update", errors: "Could not update record to data source"));

                return Ok(ResponseMessage.Message("Updated successfully", data: ""));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Admin, Client")]
        [HttpDelete]
        [Route("{id}/delete-address")]
        public async Task<IActionResult> DeleteAddress(string id)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: "Id should not be null or empty or whitespace"));

            try
            {
                // get item using id
                var item = await _missionRepository.GetVerificationItemById(id);
                if (item == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: $"Item with id: {id} was not found"));

                // delete data from source
                var result = await _missionRepository.Delete(item);
                if (!result)
                    return BadRequest(ResponseMessage.Message("Failed to delte", errors: "Could not update record to data source"));

                return Ok(ResponseMessage.Message("Deleted successfully", data: ""));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Admin, Client")]
        [HttpGet]
        [Route("addresses/{page}")]
        public async Task<IActionResult> GetAllAddress(int page)
        {
            try
            {
                // fetch all addresses
                var results = await _missionRepository.GetVerificationItemsPaginated(page, perPage);
                if(results == null)
                    return NotFound(ResponseMessage.Message("Null result(s)", errors: $"No items was not found"));


                // map items fetched to items dto
                var list = new List<ItemToReturnDTO>();
                foreach (var result in results)
                {
                    var item = new ItemToReturnDTO
                    {
                        ItemId = result.ItemId,
                        Title = result.Title,
                        Description = result.Description,
                        AddedBy = null,
                        CreatedAt = result.UpdatedAt
                    };

                    list.Add(item);
                }

                //****************************************************************************************/
                page = page <= 0 ? 1 : page;
                //****************************************************************************************/

                // new dto that contains pagination details 
                var pagedItems = new PaginatedItemsToReturnDTO
                {
                    PageMetaData = Util.Paginate(page, perPage, _missionRepository.TotalCount),
                    Data = list
                };

                return Ok(ResponseMessage.Message("Items found", data: pagedItems));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Admin, Client")]
        [HttpGet]
        [Route("{id}/address")]
        public async Task<IActionResult> GetAddress(string id)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: "Id should not be null or empty or whitespace"));

            try
            {
                // get item using id
                var result = await _missionRepository.GetVerificationItemById(id);
                if (result == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: $"Item with id: {id} was not found"));


                // map items fetched to items dto
                var item = new ItemToReturnDTO
                {
                    ItemId = result.ItemId,
                    Title = result.Title,
                    Description = result.Description,
                    AddedBy = null,
                    CreatedAt = result.UpdatedAt
                };

                return Ok(ResponseMessage.Message("Item found", data: item));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("assign-mission")]
        public async Task<IActionResult> AssignMission([FromBody] MissionTOAssignDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

            try
            {
                var agent = await _agentRepository.GetAgentById(model.FieldAgentId);
                if(agent == null)
                    return NotFound(ResponseMessage.Message("Null result(s)", errors: $"Agent with Id {model.FieldAgentId} was not found"));

                // generate item id
                string missionId = "";
                Mission result = null;
                do
                {
                    missionId = Guid.NewGuid().ToString();
                    result = await _missionRepository.GetMissionById(missionId);
                } while (result != null);

                // check if verification item is already existing in the missions table
                if (await _missionRepository.IsVerificationItemAssigned(model.VerificationItemId))
                    return BadRequest(ResponseMessage.Message("Already exists", errors: "Verification item aready assigned"));

                // construct item
                var mission = new Mission
                {
                    MissionId = missionId,
                    VerificationItemId = model.VerificationItemId,
                    FieldAgentId = model.FieldAgentId
                };

                // add item
                var result2 = await _missionRepository.Add(mission);
                if (!result2)
                    return BadRequest(ResponseMessage.Message("Failed to assign", errors: "Could not add record to data source"));

                return Ok(ResponseMessage.Message("Assigned successfully", data: new { missionId }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }

        }


        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("edit-mission-assigned")]
        public async Task<IActionResult> EditMission([FromBody] MissionToEditDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

            try
            {
                // get mission using id
                var mission = await _missionRepository.GetMissionById(model.Id);
                if (mission == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: $"Mission with Id: {model.Id} is not found"));

                // only pending assignments can be edited
                if (mission.VerificationStatus == "accepted" || mission.VerificationStatus == "verified")
                    return Unauthorized(ResponseMessage.Message("Not allowed", errors: $"Mission is ongoing or completed"));

                // check if verification item is already existing in the missions table
                if(mission.VerificationItemId != model.VerificationItemId)
                {
                    if (await _missionRepository.IsVerificationItemAssigned(model.VerificationItemId))
                        return BadRequest(ResponseMessage.Message("Already exists", errors: "Verification item aready assigned"));

                    // re-assign values
                    await DeleteMission(model.Id);
                    mission.FieldAgentId = model.FieldAgentId;
                    mission.VerificationItemId = model.VerificationItemId;
                    mission.MissionId = Guid.NewGuid().ToString();

                    // update data source
                    var result = await _missionRepository.Add(mission);
                    if (!result)
                        return BadRequest(ResponseMessage.Message("Failed to update", errors: "Could not update record to data source"));

                    return Ok(ResponseMessage.Message("Updated successfully", data: mission.MissionId));
                }

                // update data source
                mission.FieldAgentId = model.FieldAgentId;
                mission.VerificationItemId = model.VerificationItemId;
                var res = await _missionRepository.Update(mission);
                if (!res)
                    return BadRequest(ResponseMessage.Message("Failed to update", errors: "Could not update record to data source"));

                return Ok(ResponseMessage.Message("Updated successfully", data: mission.MissionId));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{missionId}/delete-mission")]
        public async Task<IActionResult> DeleteMission(string missionId)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(missionId))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: "Id should not be null or empty or whitespace"));

            try
            {
                // get mission using id
                var mission = await _missionRepository.GetMissionById(missionId);
                if (mission == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: $"Mission with Id: {missionId} is not found"));

                // only pending assignments can be deleted
                if(mission.VerificationStatus != "pending")
                    return Unauthorized(ResponseMessage.Message("Not allowed", errors: $"Mission has is ongoing"));

                // delete data from MissionVerified table
                var res = await _missionRepository.GetMissionsVeriedByMissionId(missionId);
                if (res != null)
                {
                    var del = await _missionRepository.Delete(res);
                    if (!del) throw new Exception($"Failed to delete record in MissionVerified");
                } 

                // delete data from mission table
                var result = await _missionRepository.Delete(mission);
                if (!result)
                    return BadRequest(ResponseMessage.Message("Failed to delte", errors: "Could not update record to data source"));

                return Ok(ResponseMessage.Message("Deleted successfully", data: ""));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Agent")]
        [HttpPatch]
        [Route("{missionId}/edit-status/{status}")]
        public async Task<IActionResult> UpdateMissionStatus(string missionId, string status)
        {
            if(String.IsNullOrWhiteSpace(missionId) || String.IsNullOrWhiteSpace(status))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: "Id or status should not be null or empty or whitespace"));

            try
            {
                var logginUserId = _userManager.GetUserId(User);

                // check if user's account is verified
                var user = await _userManager.FindByIdAsync(logginUserId);
                if (!user.IsVerified) return Unauthorized(ResponseMessage.Message("Unverified Account!", errors: "Unverified account is not authorized to take on task"));

                // check if mission is assigned to logged-in user
                var mission = await _missionRepository.GetMissionById(missionId);
                if(mission.FieldAgentId != logginUserId)
                    return BadRequest(ResponseMessage.Message("Id mismatch", errors: $"Mission with id: {missionId} is not assigned to logged-in user"));

                // update status
                var result = await _missionRepository.ChangeMissionStatus(status, missionId);
                if(!result)
                    return BadRequest(ResponseMessage.Message("Failed to update", errors: "Could not update record to data source"));

                return Ok(ResponseMessage.Message("Updated successfully", data: ""));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Admin, Agent")]
        [HttpGet]
        [Route("{agentId}/{status}/{page}")]
        public async Task<IActionResult> GetMissions(string agentId, string status, int page)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(agentId) || String.IsNullOrWhiteSpace(status))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: "Id or status should not be null or empty or whitespace"));

            try
            {
                //var loggedInUserId = _userManager.GetUserId(User);
                var results = await _missionRepository.GetMissionsPaginated(page, perPage, status);
                if (results == null) return NotFound(ResponseMessage.Message("Null result", errors: "No result(s) found"));

                // filter only agent's results
                var agentsMissions = new List<Mission>();
                foreach (var mission in results)
                {
                    if (mission.FieldAgentId == agentId)
                        agentsMissions.Add(mission);
                }


                // map items fetched to items dto
                var list = new List<MissionToReturn>();
                foreach (var result in agentsMissions)
                {
                    var item = new MissionToReturn
                    {
                        MissionId = result.MissionId,
                        ItemId = result.VerificationItemId,
                        Title = result.VerificationItem.Title,
                        Description = result.VerificationItem.Description,
                        AddedBy = null,
                        CreatedAt = result.UpdatedAt
                    };

                    list.Add(item);
                }

                //****************************************************************************************/
                page = page <= 0 ? 1 : page;
                //****************************************************************************************/

                // new dto that contains pagination details 
                var pagedItems = new PaginatedItemsToReturnDTO
                {
                    PageMetaData = Util.Paginate(page, perPage, _missionRepository.TotalCount),
                    Data = list
                };

                return Ok(ResponseMessage.Message("Missions found", data: pagedItems));

            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }


        [Authorize(Roles = "Agent")]
        [HttpPost]
        [Route("submit")]
        public async Task<IActionResult> SubmitMission(MissionToVerifyDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));
            try
            {
                // check if user's account is verified
                var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
                if (!user.IsVerified) return Unauthorized(ResponseMessage.Message("Unverified Account!", errors: "Unverified account is not authorized to take on task"));

                // only accepted missions can be verified
                var mission = await _missionRepository.GetMissionById(model.MissionId);
                if(mission == null) if (mission == null) 
                        return NotFound(ResponseMessage.Message("Null result", errors: $"Mission with id: {model.MissionId} was not found"));
                if(mission.VerificationStatus != "accepted") return Unauthorized(ResponseMessage.Message("Not allowed", errors: "Mission is not accepted yet"));

                // generate item id
                string Id = "";
                MissionVerified result = null;
                do
                {
                    Id = Guid.NewGuid().ToString();
                    result = await _missionRepository.GetMissionVeriedById(Id);
                } while (result != null);

                // construct item
                var missionVerified = new MissionVerified
                {
                    Id = Id,
                    MissionId = model.MissionId,
                    BuildingTypeId = model.BuildingTypeId,
                    Landmark = model.Landmark,
                    BusStop = model.BusStop,
                    BuildingColor = model.BuildingColor,
                    AddressExists = model.AddressExists,
                    TypeOfStructure = model.TypeOfStructure,
                    Longitude = model.Longitude,
                    Latitude = model.Latitude,
                    Remarks = model.Remarks
                };

                var result2 = await _missionRepository.Add(missionVerified);
                if(!result2)
                    return BadRequest(ResponseMessage.Message("Failed to submit", errors: "Could not submit record to data source"));

                if(!await _missionRepository.ChangeMissionStatus("verified", mission.MissionId))
                {
                    await _missionRepository.Delete(missionVerified);
                    throw new Exception("Could not verify mission");
                }
                return Ok(ResponseMessage.Message("Submited successfully", data: new { Id }));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }
        }
    }
}
