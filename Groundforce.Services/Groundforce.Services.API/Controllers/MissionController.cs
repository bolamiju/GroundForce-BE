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
            perPage = Convert.ToInt32(configrutation.GetSection("PhotoSettings:Size").Get<string>());
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
                    Title = model.Title,
                    Description = model.Description
                };

                // add item
                if(!await _missionRepository.Add(model))
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
                // generate item id
                string misssionId = "";
                Mission result = null;
                do
                {
                    misssionId = Guid.NewGuid().ToString();
                    result = await _missionRepository.GetMissionByIdForAgent(model.FieldAgentId, misssionId);
                } while (result != null);

                // construct item
                var mission = new Mission
                {
                    MissionId = misssionId,
                    VerificationItemId = model.VerificationItemId
                };

                // add item
                var result2 = await _missionRepository.Add(model);
                if (!result2)
                    return BadRequest(ResponseMessage.Message("Failed to assign", errors: "Could not add record to data source"));

                return Ok(ResponseMessage.Message("Assigned successfully", data: new { misssionId }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: "Could not access record from data source, error written to log file"));
            }

        }


        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("edit-mission")]
        public async Task<IActionResult> EditMission([FromBody] MissionToEditDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(model.Id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: "Id should not be null or empty or whitespace"));

            try
            {
                // get item using id
                var mission = await _missionRepository.GetMissionByIdForAgent(model.FieldAgentId, model.Id);
                if (mission == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: $"Item with id: {model.Id} was not found"));

                // re-assign values
                mission.FieldAgentId = model.FieldAgentId;
                mission.VerificationItemId = model.VerificationItemId;

                // update data source
                var result = await _missionRepository.Update(mission);
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


        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{id}/delete-mission")]
        public async Task<IActionResult> DeleteMission(string id)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: "Id should not be null or empty or whitespace"));

            try
            {
                // get item using id
                var mission = await _missionRepository.GetVerificationItemById(id);
                if (mission == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: $"Item with id: {id} was not found"));

                // delete data from source
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
        [Route("{missionId}/{status}")]
        public async Task<IActionResult> UpdateMissionStatus(string missionId, string status)
        {
            if(String.IsNullOrWhiteSpace(missionId) || String.IsNullOrWhiteSpace(status))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: "Id or status should not be null or empty or whitespace"));

            try
            {
                // check if mission is assigned to logged-in agent
                var logginUserId = _userManager.GetUserId(User);
                if (logginUserId == null) return Unauthorized(ResponseMessage.Message("Access denied", errors: $"User must be logged in"));

                var mission = await _missionRepository.GetMissionByIdForAgent(logginUserId, missionId);
                if(mission.MissionId != missionId)
                    return BadRequest(ResponseMessage.Message("Id mismatch", errors: $"Mission with id: {missionId} is not assigned to logged-in user"));

                // check if user's account is verified
                var user = await _userManager.FindByIdAsync(logginUserId);
                if (!user.IsVerified) return Unauthorized(ResponseMessage.Message("Unverified Account!", errors: "Unverified account is not authorized to take on task"));

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
            if (String.IsNullOrWhiteSpace(agentId))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: "Id should not be null or empty or whitespace"));

            try
            {
                //var loggedInUserId = _userManager.GetUserId(User);
                var missions = await _missionRepository.GetMissionsForAgentPaginated(page, perPage, agentId, status);
                if (missions == null) return NotFound(ResponseMessage.Message("Null result", errors: "No result(s) found"));


                // map items fetched to items dto
                var list = new List<ItemToReturnDTO>();
                foreach (var result in missions)
                {
                    var item = new ItemToReturnDTO
                    {
                        ItemId = result.MissionId,
                        Title = result.VerificationItem.Title,
                        Description = result.VerificationItem.Description,
                        AddedBy = null,
                        CreatedAt = result.UpdatedAt
                    };                    
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
