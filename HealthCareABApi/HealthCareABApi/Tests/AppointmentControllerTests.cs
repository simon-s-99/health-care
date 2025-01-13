using HealthCareABApi.Controllers;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HealthCareABApi.Tests
{
    public class AppointmentControllerTests
    {

        //[Fact]
        //public async Task CreateAppointment_ReturnsCreatedResult_WhenEverythingIsInOrder()
        //{
        //    // Arrange
        //    var mockService = new Mock<IAppointmentService>();

        //    var controller = new AppointmentController(mockService.Object);

        //    CreateAppointmentDTO mockDto = new CreateAppointmentDTO();
        //    Appointment appointment = new Appointment();
        //    mockService.Setup(svc => svc.CreateAppointmentAsync(mockDto)).ReturnsAsync(appointment);

        //    var result = await controller.CreateAppointment(mockDto);

        //    // Assert
        //    var createdResult = Assert.IsType<CreatedResult>(result);
        //    Assert.Equal(201, createdResult.StatusCode);
        //    mockService.Verify(svc => svc.CreateAppointmentAsync(mockDto), Times.Once);
        //}

        [Fact]
        public async Task GetAppointmentById_ReturnsOkResult_WhenAppointmentExists()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            Appointment appointment = new Appointment
            {
                Id = "6785371d6caf0d38580eb653",
                PatientId = "678523516caf0d38580eb536",
                CaregiverId = "678523516caf0d38580eb537",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            mockService.Setup(svc => svc.GetAppointmentByIdAsync(appointment.Id)).ReturnsAsync(appointment);

            // Act
            var result = await controller.GetAppointmentById(appointment.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            mockService.Verify(svc => svc.GetAppointmentByIdAsync(appointment.Id), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsNotFoundResult_WhenAppointmentDoesNotExists()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            string nonexistentId = "sometingnotfound";

            mockService.Setup(svc => svc.GetAppointmentByIdAsync(nonexistentId)).ReturnsAsync((Appointment)null);

            // Act
            var result = await controller.GetAppointmentById(nonexistentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            mockService.Verify(svc => svc.GetAppointmentByIdAsync(nonexistentId), Times.Once);
        }
    }
}