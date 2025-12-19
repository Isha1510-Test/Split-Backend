using Microsoft.AspNetCore.Mvc;
using ExpenseSharing.Models;
using ExpenseSharing.Services;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly GroupService _groupService;

        public GroupsController(GroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetAllGroups()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            return Ok(group);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<GroupDto>>> GetGroupsByUserId(int userId)
        {
            var groups = await _groupService.GetGroupsByUserIdAsync(userId);
            return Ok(groups);
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(GroupDto groupDto)
        {
            try
            {
                var createdGroup = await _groupService.CreateGroupAsync(groupDto);
                return CreatedAtAction(nameof(GetGroup), new { id = createdGroup.Id }, createdGroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var result = await _groupService.DeleteGroupAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}