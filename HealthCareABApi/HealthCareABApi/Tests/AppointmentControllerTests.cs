using HealthCareABApi.Controllers;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace HealthCareABApi.Tests
{
    public class AppointmentControllerTests
    {
        [Fact]
        public async Task CreateAppointment_ReturnsCreatedResult_WhenDTOIsValid()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);


            // Mock the logged in user/patient
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "678523516caf0d38580eb536"),
                    new Claim(ClaimTypes.Role, Roles.User)
                }
                ))
            };

            CreateAppointmentDTO dto = new CreateAppointmentDTO
            {
                PatientId = "678523516caf0d38580eb536",
                CaregiverId = "678523516caf0d38580eb537",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            mockService.Setup(svc => svc.CreateAppointmentAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.CreateAppointment(dto);

            // Assert
            var createdResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            mockService.Verify(svc => svc.CreateAppointmentAsync(dto), Times.Once);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsBadRequestResult_WhenDTOIsIncomplete()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            // CaregiverID missing
            CreateAppointmentDTO dto = new CreateAppointmentDTO
            {
                PatientId = "678523516caf0d38580eb536",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            // Mock is not reqired as the test will never hit it

            // Act
            var result = await controller.CreateAppointment(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }


        [Fact]
        public async Task GetAppointmentById_ReturnsOk_WhenAppointmentExistsAndUserIsAuthorized()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            var appointmentId = "678523516caf0d38580eb536";
            var userId = "678523516caf0d38580eb537";
            var appointment = new Appointment
            {
                Id = appointmentId,
                PatientId = userId,
                CaregiverId = "anotherCaregiverId",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            mockService
                .Setup(svc => svc.GetAppointmentByIdAsync(appointmentId))
                .ReturnsAsync(appointment);

            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, Roles.User)
                }))
            };

            // Act
            var result = await controller.GetAppointmentById(appointmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAppointment = Assert.IsType<Appointment>(okResult.Value);
            Assert.Equal(appointmentId, returnedAppointment.Id);
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            var appointmentId = "nonexistentId";

            mockService
                .Setup(svc => svc.GetAppointmentByIdAsync(appointmentId))
                .ReturnsAsync((Appointment)null);

            // Act
            var result = await controller.GetAppointmentById(appointmentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact]
        public async Task GetAppointmentById_ReturnsForbid_WhenUserIsUnauthorized()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            var appointmentId = "678523516caf0d38580eb536";
            var userId = "678523516caf0d38580eb537";
            var appointment = new Appointment
            {
                Id = appointmentId,
                PatientId = "anotherPatientId", // Different from the logged-in user
                CaregiverId = "678523516caf0d38580eb538",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            mockService
                .Setup(svc => svc.GetAppointmentByIdAsync(appointmentId))
                .ReturnsAsync(appointment);

            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, Roles.User)
                }))
            };

            // Act
            var result = await controller.GetAppointmentById(appointmentId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetAllAppointmentsByUserIdAsync_ReturnsFilteredAppointmentsByDate()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            var userId = "678523516caf0d38580eb537";
            var filterDate = DateTime.Now.Date;

            var appointments = new List<Appointment>
        {
        new Appointment
        {
            Id = "1",
            PatientId = userId,
            CaregiverId = "caregiver1",
            DateTime = filterDate.AddHours(10), // Matches filter
            Status = AppointmentStatus.Scheduled
        },
        new Appointment
        {
            Id = "2",
            PatientId = userId,
            CaregiverId = "caregiver2",
            DateTime = filterDate.AddDays(1), // Does not match filter
            Status = AppointmentStatus.Completed
        }
        };

            mockService.Setup(svc => svc.GetAllAppointmentsByUserIdAsync(userId, filterDate, true))
                       .ReturnsAsync(appointments.Where(a => a.DateTime.Date == filterDate).ToList());

            // Act
            var result = await controller.GetAllAppointmentsByUserIdAsync(userId, filterDate, true);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAppointments = Assert.IsType<List<Appointment>>(okResult.Value);
            Assert.Single(returnedAppointments);
            Assert.Equal("1", returnedAppointments[0].Id);
        }






    }
}