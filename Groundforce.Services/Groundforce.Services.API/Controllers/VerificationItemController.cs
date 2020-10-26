using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Groundforce.Common.Utilities.Helpers;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VerificationItemController : ControllerBase
    {
        private readonly ILogger<VerificationItemController> _logger;
        private readonly IVerificationItemRepository _ItemRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAgentRepository _agentRepo;
        private readonly int perPage = 10;

        public VerificationItemController(ILogger<VerificationItemController> logger, 
            IVerificationItemRepository ItemRepo, UserManager<ApplicationUser> userManager, 
            IAgentRepository agentRepo)
        {
            _logger = logger;
            _ItemRepo = ItemRepo;
            _userManager = userManager;
            _agentRepo = agentRepo;
        }

        [Authorize(Roles = "Admin, Client")]
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] ItemDTO ItemToAdd)
        {
            if (ModelState.IsValid)
            {
                var LoggedInUser = await _userManager.GetUserAsync(User);

                if (LoggedInUser == null) return NotFound("Can't access loggedIn user");

                // get item id
                string itemId = "";
                VerificationItem result = null;
                do
                {
                    itemId = Guid.NewGuid().ToString();
                    result = await _ItemRepo.GetItemById(itemId);
                } while (result != null);


                var newItem = new VerificationItem
                {
                    ItemId = itemId,
                    ApplicationUserId = LoggedInUser.Id,
                    ItemName = ItemToAdd.ItemName
                };

                try
                {
                    await _ItemRepo.AddItem(newItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException.Message);
                    return BadRequest("Failed to add the address");
                }

                // get address to return on creation
                var fetchedItem = await _ItemRepo.GetItemById(newItem.ItemId);

                if (fetchedItem != null)
                {
                    var itemToReturn = new ItemToReturnDTO
                    {
                        ItemId = fetchedItem.ItemId,
                        ItemName = fetchedItem.ItemName,
                        AddedBy = fetchedItem.ApplicationUserId,
                        CreatedAt = fetchedItem.CreatedAt
                    };

                    return Ok(itemToReturn);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = "Admin, Client")]

        public async Task<IActionResult> GetItem(string Id)
        {
            try
            {
                var agent = await _agentRepo.GetAgentById(Id);
                var fetchedItem = await _ItemRepo.GetItemById(Id);

                var itemToReturn = new ItemToReturnDTO
                {
                    ItemId = fetchedItem.ItemId,
                    ItemName = fetchedItem.ItemName,
                    AddedBy = fetchedItem.ApplicationUserId,
                    CreatedAt = fetchedItem.CreatedAt
                };

                return Ok(itemToReturn);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("items/{page}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> GetAllItems(int page)
        {
            // Fetch paginated result
            IEnumerable<VerificationItem> allItems;
            try
            {
                allItems = await _ItemRepo.GetItemsPaginated(page, perPage);
            }
            catch (Exception)
            {
                return BadRequest("Failed to fetch items");
            }

            if (allItems == null) return NotFound("No items found");

            //  Reshape result
            var itemsList = new List<ItemToReturnDTO>();
            foreach (var fetchedItem in allItems)
            {
                var itemToReturn = new ItemToReturnDTO
                {
                    ItemId = fetchedItem.ItemId,
                    ItemName = fetchedItem.ItemName,
                    AddedBy = fetchedItem.ApplicationUserId,
                    CreatedAt = fetchedItem.CreatedAt
                };

                itemsList.Add(itemToReturn);
            }

            page = page > _ItemRepo.TotalNumberOfItems? _ItemRepo.TotalNumberOfItems : page;
            page = page <= 0 ? 1 : page;

            // new dto that contains pagination details 
            var pagedItems = new PaginatedItemsToReturnDTO
            {
                PageMetaData = Util.Paginate(page, perPage, _ItemRepo.TotalNumberOfItems),
                Data = itemsList
            };

            return Ok(pagedItems);
        }

        [Authorize(Roles = "Admin, Client")]
        [HttpPatch("{Id}/Edit")]
        public async Task<IActionResult> EditItem(string Id, [FromBody] ItemDTO ItemToEdit)
        {
            // does item exists
            var item = await _ItemRepo.GetItemById(Id);
            if (item == null) return NotFound("Item could not be found!");

            if (ModelState.IsValid)
            {
                var LoggedInUser = await _userManager.GetUserAsync(User);

                if (LoggedInUser == null) return NotFound("Can't access loggedIn user");

                item.ItemId = Id;
                item.ItemName = ItemToEdit.ItemName;
                item.ApplicationUserId = LoggedInUser.Id;
                item.CreatedAt = item.CreatedAt;

                try
                {
                    await _ItemRepo.UpdateItem(item);
                    return Ok("Address editted");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest("Failed to update");
                }

            }
            return BadRequest(ModelState);
        }

    }
}
