using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Services.Implementations;
using HealthCareABApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetCaregiverById([FromQuery] string id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                GetUserDTO dto = new GetUserDTO
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    Phonenumber = user.Phonenumber,
                    Username = user.Username,
                    Roles = user.Roles
                };
                return Ok(dto);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
