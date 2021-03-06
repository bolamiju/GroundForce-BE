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


        [Authorize(Roles = "admin, client")]
        [HttpPost]
        [Route("add-address")]
        public async Task<IActionResult> AddAddress([FromBody] ItemToAddDTO model)
        {
            try
            {
                // ensure model state is valid
                if (!ModelState.IsValid)
                    return BadRequest(ResponseMessage.Message("Model state error", errors:  ModelState));

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

                // generate item id
                string itemId = "";
                VerificationItem result;
                do
                {
                    itemId = Guid.NewGuid().ToString();
                    result = await _missionRepository.GetVerificationItemById(itemId);
                } while (result != null);

                // construct item
                var newItem = new VerificationItem
                {
                    ItemId = itemId,
                    ApplicationUserId = user.Id,
                    UpdatedBy = user.Id,
                    Title = model.Title,
                    Description = model.Description
                };

                // add item
                if(!await _missionRepository.Add(newItem))
                    return BadRequest(ResponseMessage.Message("Failed to add", errors: new { message = "Could not add record to data source" }));

                return Ok(ResponseMessage.Message("Added successfully", data: new { AddressId = newItem.ItemId }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin, client")]
        [HttpPut]
        [Route("edit-address")]
        public async Task<IActionResult> EditAddress([FromBody] ItemToEditDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

            // check if id is not null or empty
            if(String.IsNullOrWhiteSpace(model.Id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Id should not be null or empty or whitespace" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            try
            {
                // get item using id
                var item = await _missionRepository.GetVerificationItemById(model.Id);
                if (item == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: new { message = $"Item with id: {model.Id} was not found" }));

                // re-assign values
                item.Title = model.Title;
                item.Description = model.Description;
                item.UpdatedBy = user.Id;
                item.UpdatedAt = DateTime.Now;


                // update data source
                var result = await _missionRepository.Update(item);
                if (!result)
                    return BadRequest(ResponseMessage.Message("Failed to update", errors: new { message = "Could not update record to data source" }));

                return Ok(ResponseMessage.Message("Updated successfully", data: new { message = "Update was successful" }));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin, client")]
        [HttpDelete]
        [Route("{id}/delete-address")]
        public async Task<IActionResult> DeleteAddress(string id)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Id should not be null or empty or whitespace" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            try
            {
                // get item using id
                var item = await _missionRepository.GetVerificationItemById(id);
                if (item == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: new { message = $"Item with id: {id} was not found" }));

                // delete data from source
                var result = await _missionRepository.Delete(item);
                if (!result)
                    return BadRequest(ResponseMessage.Message("Failed to delete", errors: new { message = "Could not update record to data source" }));

                return Ok(ResponseMessage.Message("Deleted successfully", data: new { message = "Address was deleted sucessfully" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin, client")]
        [HttpGet]
        [Route("addresses/{page}")]
        public async Task<IActionResult> GetAllAddress(int page)
        {
            try
            {
                // fetch all addresses
                var results = await _missionRepository.GetVerificationItemsPaginated(page, perPage);
                if(results == null)
                    return NotFound(ResponseMessage.Message("Null result(s)", errors: new { message = $"No items was not found" }));


                // map items fetched to items dto
                var list = new List<ItemToReturnDTO>();
                foreach (var result in results)
                {
                    var item = new ItemToReturnDTO
                    {
                        ItemId = result.ItemId,
                        Title = result.Title,
                        Description = result.Description,
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
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin, client")]
        [HttpGet]
        [Route("{id}/address")]
        public async Task<IActionResult> GetAddress(string id)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Id should not be null or empty or whitespace" }));

            try
            {
                // get item using id
                var result = await _missionRepository.GetVerificationItemById(id);
                if (result == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: new { message = $"Item with id: {id} was not found" }));


                // map items fetched to items dto
                var item = new ItemToReturnDTO
                {
                    ItemId = result.ItemId,
                    Title = result.Title,
                    Description = result.Description,
                    CreatedAt = result.UpdatedAt
                };

                return Ok(ResponseMessage.Message("Item found", data: item));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("assign-mission")]
        public async Task<IActionResult> AssignMission([FromBody] MissionTOAssignDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            try
            {
                var agent = await _agentRepository.GetAgentById(model.FieldAgentId);
                if(agent == null)
                    return NotFound(ResponseMessage.Message("Null result(s)", errors: new { message = $"Agent with Id {model.FieldAgentId} was not found" }));

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
                    return BadRequest(ResponseMessage.Message("Already exists", errors: new { message = "Verification item aready assigned" }));

                // construct item
                var mission = new Mission
                {
                    MissionId = missionId,
                    VerificationItemId = model.VerificationItemId,
                    FieldAgentId = model.FieldAgentId,
                    AddedBy = user.Id,
                    UpdatedBy = user.Id
                };

                // add item
                var result2 = await _missionRepository.Add(mission);
                if (!result2)
                    return BadRequest(ResponseMessage.Message("Failed to assign", errors: new { message = "Could not add record to data source" }));

                return Ok(ResponseMessage.Message("Assigned successfully", data: new { missionId }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("edit-mission-assigned")]
        public async Task<IActionResult> EditMission([FromBody] MissionToEditDTO model)
        {
            // return error if model state is not valid
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Model state error", errors: ModelState));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            try
            {
                // get mission using id
                var mission = await _missionRepository.GetMissionById(model.Id);
                if (mission == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: new { message = $"Mission with Id: {model.Id} is not found" }));

                // only pending assignments can be edited
                if (mission.VerificationStatus == "accepted" || mission.VerificationStatus == "verified")
                    return Unauthorized(ResponseMessage.Message("Not allowed", errors: new { message = $"Mission is ongoing or completed" }));

                // check if verification item is already existing in the missions table
                if(mission.VerificationItemId != model.VerificationItemId)
                {
                    if (await _missionRepository.IsVerificationItemAssigned(model.VerificationItemId))
                        return BadRequest(ResponseMessage.Message("Already exists", errors: new { message = "Verification item aready assigned" }));

                    // re-assign values
                    await DeleteMission(model.Id);
                    mission.FieldAgentId = model.FieldAgentId;
                    mission.VerificationItemId = model.VerificationItemId;
                    mission.MissionId = Guid.NewGuid().ToString();
                    mission.AddedBy = user.Id;
                    mission.UpdatedBy = user.Id;

                    // update data source
                    var result = await _missionRepository.Add(mission);
                    if (!result)
                        return BadRequest(ResponseMessage.Message("Failed to update", errors: new { message = "Could not update record to data source" }));

                    return Ok(ResponseMessage.Message("Updated successfully", data: new { message = mission.MissionId }));
                }

                // update data source
                mission.FieldAgentId = model.FieldAgentId;
                mission.VerificationItemId = model.VerificationItemId;
                mission.UpdatedBy = user.Id;
                var res = await _missionRepository.Update(mission);
                if (!res)
                    return BadRequest(ResponseMessage.Message("Failed to update", errors: new { message = "Could not update record to data source" }));

                return Ok(ResponseMessage.Message("Updated successfully", data: new { message = mission.MissionId }));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{missionId}/delete-mission")]
        public async Task<IActionResult> DeleteMission(string missionId)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(missionId))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Id should not be null or empty or whitespace" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            try
            {
                // get mission using id
                var mission = await _missionRepository.GetMissionById(missionId);
                if (mission == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: new { message = $"Mission with Id: {missionId} is not found" }));

                // only pending assignments can be deleted
                if(mission.VerificationStatus != "pending")
                    return Unauthorized(ResponseMessage.Message("Not allowed", errors: new { message = $"Mission has is ongoing" }));

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
                    return BadRequest(ResponseMessage.Message("Failed to delete", errors: new { message = "Could not update record to data source" }));

                return Ok(ResponseMessage.Message("Deleted successfully", data: new { message = "Deleted sucessfully" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "agent")]
        [HttpPatch]
        [Route("{missionId}/acceptance-status/{status}")]
        public async Task<IActionResult> UpdateMissionStatus(string missionId, string status)
        {
            if(String.IsNullOrWhiteSpace(missionId) || String.IsNullOrWhiteSpace(status))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: new { message = "Id or status should not be null or empty or whitespace" }));

            try
            {
                var logginUserId = _userManager.GetUserId(User);

                // check if user's account is verified
                var user = await _userManager.FindByIdAsync(logginUserId);
                if (!user.IsVerified) return Unauthorized(ResponseMessage.Message("Unverified Account!", errors: new { message = "Unverified account is not authorized to take on task" }));

                // check if mission is assigned to logged-in user
                var mission = await _missionRepository.GetMissionById(missionId);
                if(mission.FieldAgentId != logginUserId)
                    return BadRequest(ResponseMessage.Message("Id mismatch", errors: new { message = $"Mission with id: {missionId} is not assigned to logged-in user" }));

                // update status
                var result = await _missionRepository.ChangeMissionStatus(status, missionId, logginUserId);
                if(!result)
                    return BadRequest(ResponseMessage.Message("Failed to update", errors: new { message = "Could not update record to data source" }));

                return Ok(ResponseMessage.Message("Updated successfully", data: new { message = "Update was sucessful" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "admin, agent")]
        [HttpGet]
        [Route("{agentId}/{status}/{page}")]
        public async Task<IActionResult> GetMissions(string agentId, string status, int page)
        {
            // check if id is not null or empty
            if (String.IsNullOrWhiteSpace(agentId) || String.IsNullOrWhiteSpace(status))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: new { message = "Id or status should not be null or empty or whitespace" }));

            try
            {
                //var loggedInUserId = _userManager.GetUserId(User);
                var results = await _missionRepository.GetMissionsPaginated(page, perPage, status);
                if (results == null) return NotFound(ResponseMessage.Message("Null result", errors: new { message = "No result(s) found" }));

                // filter only agent's results
                var agentsMissions = new List<Mission>();
                foreach (var mission in results)
                {
                    if (mission.FieldAgentId == agentId)
                        agentsMissions.Add(mission);
                }


                // map items fetched to items dto
                var list = new List<MissionToReturnDTO>();
                foreach (var result in agentsMissions)
                {
                    var item = new MissionToReturnDTO
                    {
                        MissionId = result.MissionId,
                        ItemId = result.VerificationItemId,
                        Title = result.VerificationItem.Title,
                        Description = result.VerificationItem.Description,
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
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }


        [Authorize(Roles = "agent")]
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
                if (!user.IsVerified) return Unauthorized(ResponseMessage.Message("Unverified Account!", errors: new { message = "Unverified account is not authorized to take on task" }));

                // only accepted missions can be verified
                var mission = await _missionRepository.GetMissionById(model.MissionId);
                if(mission == null)
                        return NotFound(ResponseMessage.Message("Null result", errors: new { message = $"Mission with id: {model.MissionId} was not found" }));
                if(mission.VerificationStatus != "accepted") return Unauthorized(ResponseMessage.Message("Not allowed", errors: new { message = "Mission is not accepted yet" }));

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
                    Latitude = model.Latitude
                };

                var result2 = await _missionRepository.Add(missionVerified);
                if(!result2)
                    return BadRequest(ResponseMessage.Message("Failed to submit", errors: new { message = "Could not submit record to data source" }));

                if(!await _missionRepository.ChangeMissionStatus("verified", mission.MissionId, user.Id))
                {
                    await _missionRepository.Delete(missionVerified);
                    throw new Exception("Could not verify mission");
                }

                return Ok(ResponseMessage.Message("Submited successfully", data: new { Id }));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }

        #region ONLY FOR DEVELOPMENT PURPOSE
        [HttpGet]
        [Route("building-types/{page}")]
        public async Task<IActionResult> GetBuildingTypes(int page)
        {
            try
            {
                var buildingTypes = await _missionRepository.GetAllBuildingTypesPaginated(page, perPage);
                if (buildingTypes == null)
                    return NotFound(ResponseMessage.Message("Null result", errors: new { message = $"No result found" }));

                List<BuildingTypeToReturnDTO> list = new List<BuildingTypeToReturnDTO>();
                foreach (var item in buildingTypes)
                {
                    list.Add(new BuildingTypeToReturnDTO { TypeId = item.TypeId, TypeName = item.TypeName });
                }

                page = page <= 0 ? 1 : page;

                var pagedResult = new PaginatedItemsToReturnDTO
                {
                    PageMetaData = Util.Paginate(page, perPage, _missionRepository.TotalCount),
                    Data = list
                };

                return Ok(ResponseMessage.Message("BuildingTypes found", data: pagedResult));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }
        #endregion
    }
}
