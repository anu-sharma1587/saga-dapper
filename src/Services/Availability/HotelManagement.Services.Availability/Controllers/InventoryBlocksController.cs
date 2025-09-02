using HotelManagement.Services.Availability.Models;
using HotelManagement.Services.Availability.Models.Dtos;
using HotelManagement.Services.Availability.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Services.Availability.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryBlocksController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<InventoryBlocksController> _logger;

    public InventoryBlocksController(IAvailabilityService availabilityService, ILogger<InventoryBlocksController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<InventoryBlock>>> GetInventoryBlocks([FromQuery] Guid hotelId, [FromQuery] Guid? roomTypeId = null)
    {
        try
        {
            var blocks = await _availabilityService.GetActiveInventoryBlocksAsync(hotelId, roomTypeId);
            return Ok(blocks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory blocks for hotel {HotelId}", hotelId);
            return StatusCode(500, new { message = "An error occurred while retrieving inventory blocks" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<InventoryBlock>> CreateInventoryBlock([FromBody] CreateInventoryBlockRequest request)
    {
        try
        {
            var block = new InventoryBlock
            {
                HotelId = request.HotelId,
                RoomTypeId = request.RoomTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                BlockedRooms = request.BlockedRooms,
                Reason = request.Reason,
                Reference = request.Reference
            };

            var createdBlock = await _availabilityService.CreateInventoryBlockAsync(block);
            return CreatedAtAction(nameof(GetInventoryBlocks), new { hotelId = block.HotelId }, createdBlock);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory block for hotel {HotelId}", request.HotelId);
            return StatusCode(500, new { message = "An error occurred while creating inventory block" });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateInventoryBlock(Guid id, [FromBody] UpdateInventoryBlockRequest request)
    {
        try
        {
            var block = new InventoryBlock
            {
                Id = id,
                HotelId = request.HotelId,
                RoomTypeId = request.RoomTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                BlockedRooms = request.BlockedRooms,
                Reason = request.Reason,
                Reference = request.Reference,
                IsActive = request.IsActive
            };

            var success = await _availabilityService.UpdateInventoryBlockAsync(block);
            if (success)
            {
                return Ok();
            }

            return NotFound($"Inventory block with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory block {BlockId}", id);
            return StatusCode(500, new { message = "An error occurred while updating inventory block" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteInventoryBlock(Guid id)
    {
        try
        {
            var success = await _availabilityService.DeleteInventoryBlockAsync(id);
            if (success)
            {
                return NoContent();
            }

            return NotFound($"Inventory block with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inventory block {BlockId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting inventory block" });
        }
    }
}
