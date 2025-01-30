using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Services.Implementations;
using HealthCareABApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById([FromQuery] string id)
        {
            try
            {
                // Get the logged in users role and identifier
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindFirst(ClaimTypes.Role)?.Value;

                // If a user (patient) want to access a user
                if (!roles.Contains("Admin"))
                {
                    var userUser = await _userService.GetUserByIdAsync(id);

                    if (userId != id)
                    {
                        // If the accessed user is not themselves or a caregiver, return forbidden
                        if (!userUser.Roles.Contains("Admin"))
                        {
                            return Forbid("You do not have access to this user.");
                        }

                        GetUserDTO userDto = new GetUserDTO
                        {
                            Firstname = userUser.Firstname,
                            Lastname = userUser.Lastname,
                            Email = userUser.Email,
                            Phonenumber = userUser.Phonenumber,
                            Username = userUser.Username,
                            Roles = userUser.Roles
                        };
                        return Ok(userDto);
                    }
                }

                // If admin return user
                var adminUser = await _userService.GetUserByIdAsync(id);

                GetUserDTO adminDto = new GetUserDTO
                {
                    Firstname = adminUser.Firstname,
                    Lastname = adminUser.Lastname,
                    Email = adminUser.Email,
                    Phonenumber = adminUser.Phonenumber,
                    Username = adminUser.Username,
                    Roles = adminUser.Roles
                };
                return Ok(adminDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
