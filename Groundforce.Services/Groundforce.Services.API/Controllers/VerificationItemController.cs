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

                if (LoggedInUser == null) return NotFound(ResponseMessage.Message("Can't access loggedIn user"));

                // generate item id
                string itemId = "";
                VerificationItem result = null;
                do
                {
                    itemId = Guid.NewGuid().ToString();
                    result = await _ItemRepo.GetItemById(itemId);
                } while (result != null);

                // construct item
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
                    return BadRequest(ResponseMessage.Message("Failed to add the address"));
                }

                return Ok(ResponseMessage.Message("Item added!", new { newItem.ItemId }));

            }
            return BadRequest(ModelState);
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> GetItem(string Id)
        {
            // get logged-in user
            var LoggedInUser = await _userManager.GetUserAsync(User);

            // get item by id
            VerificationItem fetchedItem = null;
            try
            {
                fetchedItem = await _ItemRepo.GetItemById(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Failed to fetch item"));
            }

            // construct item if not null
            if (fetchedItem == null) return NotFound(ResponseMessage.Message($"Item with id {Id} was not found"));

            var itemToReturn = new ItemToReturnDTO
            {
                ItemId = fetchedItem.ItemId,
                ItemName = fetchedItem.ItemName,
                AddedBy = fetchedItem.ApplicationUserId,
                CreatedAt = fetchedItem.CreatedAt
            };

            // if user is admin then return item else only return item added by logged-in user
            if (await _userManager.IsInRoleAsync(LoggedInUser, "Admin"))
            {
                return Ok(ResponseMessage.Message("Item found", itemToReturn));
            }
            else if (LoggedInUser.Id == fetchedItem.ApplicationUser.Id)
            {
                return Ok(ResponseMessage.Message("Item found", itemToReturn));
            }
            else
            {
                return NotFound(ResponseMessage.Message("Could not find item as added by client"));
            }
        }

        [HttpGet("items/{page}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> GetAllItems(int page)
        {
            var LoggedInUser = await _userManager.GetUserAsync(User);

            // Fetch paginated result
            IEnumerable<VerificationItem> allItems;
            try
            {
                allItems = await _ItemRepo.GetItemsPaginated(page, perPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Failed to fetch items"));
            }

            if (allItems == null) return NotFound(ResponseMessage.Message("No items found"));

            //  Reshape result
            var itemsList = new List<ItemToReturnDTO>();
            if(await _userManager.IsInRoleAsync(LoggedInUser, "Admin"))
            {
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
            }
            else
            {
                foreach (var fetchedItem in allItems)
                {
                    if(fetchedItem.ApplicationUserId == LoggedInUser.Id)
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
                }
            }

            page = page > _ItemRepo.TotalNumberOfItems? _ItemRepo.TotalNumberOfItems : page;
            page = page <= 0 ? 1 : page;

            // new dto that contains pagination details 
            var pagedItems = new PaginatedItemsToReturnDTO
            {
                PageMetaData = Util.Paginate(page, perPage, _ItemRepo.TotalNumberOfItems),
                Data = itemsList
            };

            return Ok(ResponseMessage.Message("Items found", pagedItems));
        }

        [Authorize(Roles = "Admin, Client")]
        [HttpPatch("{Id}/Edit")]
        public async Task<IActionResult> EditItem(string Id, [FromBody] ItemDTO ItemToEdit)
        {

            if (ModelState.IsValid)
            {
                var LoggedInUser = await _userManager.GetUserAsync(User);

                // does item exists
                var item = await _ItemRepo.GetItemById(Id);
                if (item == null) return NotFound(ResponseMessage.Message("Item could not be found!"));

                item.ItemId = Id;
                item.ItemName = ItemToEdit.ItemName;
                item.ApplicationUserId = LoggedInUser.Id;
                item.CreatedAt = item.CreatedAt;

                try
                {
                    await _ItemRepo.UpdateItem(item);
                    return Ok(ResponseMessage.Message("Address editted"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest(ResponseMessage.Message("Failed to update"));
                }

            }
            return BadRequest(ModelState);
        }

    }
}
