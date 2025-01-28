using HealthCareABApi.Controllers;
using HealthCareABApi.Models;
using HealthCareABApi.DTO;
using HealthCareABApi.Services.Interfaces;  
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace HealthCareABApiTests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsConflict_WhenUsernameExists()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();  
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            var controller = new AuthController(mockUserService.Object, mockJwtTokenService.Object);

            var registerDto = new RegisterDTO
            {
                Firstname = "Walter",
                Lastname = "White",
                Username = "existingUser",
                Email = "test@example.com",
                Phonenumber = "1234567890",
                Password = "password123",
                Roles = new List<string> { "User" }
            };

            mockUserService.Setup(s => s.ExistsByUsernameAsync(registerDto.Username))
                           .ReturnsAsync(true); 

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Username is already taken", conflictResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            var controller = new AuthController(mockUserService.Object, mockJwtTokenService.Object);

            var registerDto = new RegisterDTO
            {
                Firstname = "Jesse",
                Lastname = "Pinkman",
                Username = "newUser",
                Email = "jesse@example.com",
                Phonenumber = "9876543210",
                Password = "securePassword123",
                Roles = new List<string> { "Admin" }
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RegisterResponseDTO>(okResult.Value);

            Assert.Equal("User registered successfully", response.Message);
            Assert.Equal(registerDto.Username, response.Username);
            Assert.Equal(registerDto.Roles, response.Roles);

            mockUserService.Verify(s => s.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            var controller = new AuthController(mockUserService.Object, mockJwtTokenService.Object);

            var loginDto = new LoginDTO
            {
                Username = "validUser",
                Password = "validPassword123"
            };

            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Firstname = "Walter",
                Lastname = "White",
                Email = "walter@mail.com",
                Phonenumber = "123456789",
                Username = "validUser",
                PasswordHash = "hashedValidPassword123",
                Roles = new List<string> { "User" }
            };

            mockUserService.Setup(s => s.GetUserByUsernameAsync(loginDto.Username))
                           .ReturnsAsync(user);

            mockUserService.Setup(s => s.VerifyPassword(loginDto.Password, user.PasswordHash))
                           .Returns(true);

            mockJwtTokenService.Setup(s => s.GenerateToken(user))
                               .Returns("mockJwtToken");

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext = controllerContext;

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDTO>(okResult.Value);

            Assert.Equal("Login successful", response.Message);
            Assert.Equal(user.Username, response.Username);
            Assert.Equal(user.Roles, response.Roles);
            Assert.Equal(user.Id, response.UserId);

            mockJwtTokenService.Verify(s => s.GenerateToken(It.Is<User>(u => u.Id == user.Id)), Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
            var controller = new AuthController(mockUserService.Object, mockJwtTokenService.Object);

            var loginDto = new LoginDTO
            {
                Username = "invalidUser",
                Password = "wrongPassword"
            };

            // Simulate user not found or incorrect password
            mockUserService.Setup(s => s.GetUserByUsernameAsync(loginDto.Username))
                           .ReturnsAsync((User)null);  

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Incorrect username or password", unauthorizedResult.Value);

            // Verify that no token was generated
            mockJwtTokenService.Verify(s => s.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}
