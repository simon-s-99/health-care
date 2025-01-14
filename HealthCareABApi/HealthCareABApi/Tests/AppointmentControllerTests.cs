﻿using HealthCareABApi.Controllers;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
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
        //    mockService.Setup(svc => svc.CreateAppointmentAsync(mockDto)).Returns(appointment);

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
            var mockAppointmentService = new Mock<IAppointmentService>();

            var controller = new AppointmentController(mockAppointmentService.Object);

            Appointment appointment = new Appointment
            {
                Id = "6785371d6caf0d38580eb653",
                PatientId = "678523516caf0d38580eb536",
                CaregiverId = "678523516caf0d38580eb537",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            mockAppointmentService.Setup(svc => svc.GetAppointmentByIdAsync(appointment.Id)).ReturnsAsync(appointment);

            // Act
            var result = await controller.GetAppointmentById(appointment.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedAppointment = Assert.IsType<Appointment>(okResult.Value);
            Assert.Equal(appointment.Id, returnedAppointment.Id);
            Assert.Equal(appointment.PatientId, returnedAppointment.PatientId);
            Assert.Equal(appointment.CaregiverId, returnedAppointment.CaregiverId);
            Assert.Equal(appointment.DateTime, returnedAppointment.DateTime);
            Assert.Equal(appointment.Status, returnedAppointment.Status);
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsNotFoundResult_WhenAppointmentDoesNotExists()
        {
            // Arrange
            var mockAppointmentService = new Mock<IAppointmentService>();

            var controller = new AppointmentController(mockAppointmentService.Object);

            string nonexistentId = "sometingnotfound";

            mockAppointmentService.Setup(svc => svc.GetAppointmentByIdAsync(nonexistentId)).ReturnsAsync((Appointment)null);

            // Act
            var result = await controller.GetAppointmentById(nonexistentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            mockAppointmentService.Verify(svc => svc.GetAppointmentByIdAsync(nonexistentId), Times.Once);
        }

        //[Fact]
        //public async Task DeleteAppointment_ReturnsNotFoundResult_WhenAppointmentDoesNotExist()
        //{
        //    // Arrange
        //    var mockAppointmentService = new Mock<IAppointmentService>();

        //    var controller = new AppointmentController(mockAppointmentService.Object);

        //    string nonexistentId = "sometingnotfound";

        //    //mockAppointmentService.Setup(svc => svc.DeleteAppointmentByIdAsync(nonexistentId)).ReturnsAsync();

        //    // Act
        //    var result = await controller.GetAppointmentById(nonexistentId);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundResult>(result);
        //    Assert.Equal(404, notFoundResult.StatusCode);

        //    mockAppointmentService.Verify(svc => svc.GetAppointmentByIdAsync(nonexistentId), Times.Once);
        //}
    }
}