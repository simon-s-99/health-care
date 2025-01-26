using System.Security.Claims;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HealthCareABApi.Services.Helpers;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(UserService userService, JwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            // Check if username already exists
            if (await _userService.ExistsByUsernameAsync(request.Username))
            {
                return Conflict("Username is already taken");
            }
            // Check if email already exist
            if (await _userService.ExistsByEmailAsync(request.Email))
            {
                return Conflict("Email is already registered");
            }

            // Check if phone number already exists
            if (await _userService.ExistsByPhoneNumberAsync(request.Phonenumber))
            {
                return Conflict("Phone number is already registered");
            }

            // Create and map a User entity with hashed password and default roles if none are specified.
            var user = new User
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Email = request.Email,
                Phonenumber = request.Phonenumber,
                Username = request.Username,
                PasswordHash = _userService.HashPassword(request.Password),
                Roles = request.Roles == null || !request.Roles.Any()
                    ? new List<string> { "User" }  // Default role
                    : request.Roles
            };

            await _userService.CreateUserAsync(user);

            // Prepare response with username and roles
            var regResponse = new
            {
                message = "User registered successfully",
                username = user.Username,
                roles = user.Roles
            };

            return Ok(regResponse);
        }

        [Authorize]
        [HttpPatch("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateDTO request)
        {
            // Retrieve the user ID from JWT claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized("User is not authenticated");


            // Fetch the user from the database
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            // Update firstname
            if (!string.IsNullOrEmpty(request.Firstname))
                user.Firstname = request.Firstname;

            // Update lastname
            if (!string.IsNullOrEmpty(request.LastName))
                user.Lastname = request.LastName;

            // Email update
            if (!string.IsNullOrEmpty(request.Email))
            {
                if (!ValidationHelper.IsValidEmail(request.Email)) return BadRequest("Invalid email format");

                if (await _userService.ExistsByEmailAsync(request.Email) && request.Email != user.Email)
                    return Conflict("Email is already in use");

                user.Email = request.Email;
            }

            // Phone number
            if (!string.IsNullOrEmpty(request.Phonenumber))
            {
                if (!ValidationHelper.IsValidPhoneNumber(request.Phonenumber))
                    return BadRequest("Invalid phone number format");

                if (await _userService.ExistsByPhoneNumberAsync(request.Phonenumber) && request.Phonenumber != user.Phonenumber)
                    return Conflict("Phone number is already in use");

                user.Phonenumber = request.Phonenumber;
            }

            await _userService.UpdateUserAsync(user);

            return Ok(new
            {
                message = "User updated successfully",
                updatedUser = new
                {
                    user.Firstname,
                    user.Lastname,
                    user.Email,
                    user.Phonenumber
                }
            });



        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            // Extract the user ID or username from claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized("User is not authenticated");

            // Fetch the user from the database
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null) NotFound("User not found");


            // Verify the current password
            if (!_userService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password is incorrect");

            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest("New password and confirmation password do not match");

            if (request.NewPassword.Length < 8 || !ValidationHelper.HasComplexity(request.NewPassword))
                return BadRequest("New password must be at least 8 characters and contain a mix of uppercase, lowercase, numbers, and special characters");

            user.PasswordHash = _userService.HashPassword(request.NewPassword);
            await _userService.UpdateUserAsync(user);

            return Ok("Password changed successfully");

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            try
            {
                // Fetch user by username
                var user = await _userService.GetUserByUsernameAsync(request.Username);

                // Check if the user exists and the password matches
                if (user == null || !_userService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Incorrect username or password");
                }

                // Generate a JWT token for the authenticated user.
                var token = _jwtTokenService.GenerateToken(user);

                // Define cookie options for storing the JWT token.
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,    // Only accessible via HTTP (not JavaScript), enhancing security.
                    Secure = false,     // Set to true in production to enforce HTTPS. This should be done in production
                    Path = "/",         // Cookie available to all paths.
                    SameSite = SameSiteMode.Strict, // Prevents CSRF attacks by restricting cookie usage.
                    Expires = DateTimeOffset.Now.AddHours(10) // Cookie expiration (e.g., 10 hours from now).
                };

                // Add the JWT token to an HTTP-only cookie.
                HttpContext.Response.Cookies.Append("jwt", token, cookieOptions);

                // Prepare a response without the JWT token, including only user details and roles.
                var authResponse = new
                {
                    message = "Login successful",
                    username = user.Username,
                    roles = user.Roles,
                    userId = user.Id
                };

                return Ok(authResponse);
            }
            catch (Exception)
            {
                return Unauthorized("Authentication failed");
            }
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Clear the JWT cookie by setting it to expire immediately
            HttpContext.Response.Cookies.Append("jwt", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = false, 
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow // Expire immediately
            });

            return Ok("Logged out successfully");
        }

        // Endpoint to check if a user is authenticated.
        [Authorize] // Require authorization for this endpoint.
        [HttpGet("check")]
        public IActionResult CheckAuthentication()
        {
            // If the user is not authenticated, return an unauthorized response.
            if (!User.Identity!.IsAuthenticated)
            {
                return Unauthorized("Not authenticated");
            }

            // Extract the username from the token claims.
            var username = User.Identity.Name;

            // Extract the roles from the token claims.
            var roles = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();

            var userId = User.Claims
                            .Where(c => c.Type == ClaimTypes.NameIdentifier)
                            .Select(c => c.Value)
                            .FirstOrDefault();

            /// Return an authentication status with username and roles.
            return Ok(new
            {
                message = "Authenticated",
                username = username,
                userId = userId,
                roles = roles
            });
        }


    }
}