using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WeatherBackend.Models;
using WeatherBackend.Service;

namespace WeatherBackend.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserHistoryController : ControllerBase
    {
        private readonly UserHistoryService _userHistoryService;

        public UserHistoryController(UserHistoryService userHistoryService)
        {
            _userHistoryService = userHistoryService;
        }

        [HttpGet("{userId}")]
        [ApiVersion("1.0")]
        public async Task<ActionResult<List<UserSearchHistory>>> GetUserHistory(string userId)
        {
            try
            {
                var history = await _userHistoryService.GetUserSearchHistory(userId);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving search history" });
            }
        }

        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<UserSearchHistory>> AddSearchHistory(
            [FromBody] UserSearchHistory history)
        {
            try
            {
                
                history.Id = ObjectId.GenerateNewId().ToString();

                var addedHistory = await _userHistoryService.AddSearchHistory(history);
                return CreatedAtAction(
                    nameof(GetUserHistory),
                    new { userId = history.UserId },
                    addedHistory
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding search history" });
            }
        }

        [HttpDelete("{historyId}")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> DeleteSearchHistory(
            string historyId,
            [FromQuery] string userId)
        {
            try
            {
                await _userHistoryService.DeleteSearchHistory(historyId, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting search history" });
            }
        }

        [HttpDelete("clear/{userId}")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ClearAllUserSearchHistory(string userId)
        {
            try
            {
                await _userHistoryService.ClearAllUserSearchHistory(userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while clearing search history" });
            }
        }
    }
}