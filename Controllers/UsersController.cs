using Microsoft.AspNetCore.Mvc;
using ExpenseSharing.Models;
using ExpenseSharing.Services;
using Microsoft.EntityFrameworkCore;
using ExpenseSharing.Data;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ExpenseSharingContext _context;

        public UsersController(UserService userService, ExpenseSharingContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserDto userDto)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, UserDto userDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, userDto);
            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
            if (user == null) return Unauthorized("Invalid credentials");
            return Ok(user);
        }

        [HttpGet("test")]
        public async Task<ActionResult> TestEndpoint()
        {
            var userCount = await _context.Users.CountAsync();
            var users = await _context.Users.Take(5).Select(u => new { u.Id, u.Name, u.Email }).ToListAsync();
            return Ok(new { UserCount = userCount, Users = users });
        }

        [HttpGet("{id}/balance")]
        public async Task<ActionResult<UserBalanceDto>> GetUserBalance(int id)
        {
            try
            {
                var balance = await _userService.GetUserOverallBalanceAsync(id);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error getting user balance for user {id}: {ex.Message}");
                
                // Return a default balance instead of error
                var defaultBalance = new UserBalanceDto
                {
                    UserId = id,
                    UserName = "Unknown User",
                    TotalOwed = 0,
                    TotalOwing = 0,
                    NetBalance = 0,
                    GroupBalances = new List<GroupBalanceDto>()
                };
                
                return Ok(defaultBalance);
            }
        }
    }
}