using HealthCareABApi.Controllers;
using HealthCareABApi.Models;
using HealthCareABApi.DTO;
using HealthCareABApi.Services.Interfaces;  
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    }
}
