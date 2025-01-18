    using Microsoft.AspNetCore.Mvc;
    using UserService.Application.DTOs;
    using UserService.Application.Services;

    namespace UserService.Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class UsersController : ControllerBase
        {
            private readonly IUserAppService _userAppService;

            public UsersController(IUserAppService userAppService)
            {
                _userAppService = userAppService;
            }

            [HttpGet("{id:int}")]
            public async Task<IActionResult> Get(int id)
            {
                var user = await _userAppService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var users = await _userAppService.GetAllUsersAsync();
                return Ok(users);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] UserDto userDto)
            {
                var createdUser = await _userAppService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
            }

            [HttpPut("{id:int}")]
            public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
            {
                var updatedUser = await _userAppService.UpdateUserAsync(id, userDto);
                if (updatedUser == null)
                    return NotFound();
                return Ok(updatedUser);
            }

            [HttpDelete("{id:int}")]
            public async Task<IActionResult> Delete(int id)
            {
                var success = await _userAppService.DeleteUserAsync(id);
                if (!success)
                    return NotFound();
                return NoContent();
            }
        }
    }
