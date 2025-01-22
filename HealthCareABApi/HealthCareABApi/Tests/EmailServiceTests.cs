using Xunit;
using HealthCareABApi.Models;

namespace HealthCareABApi.Tests
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendAppointmentEmail_UnsupportedAppointmentStatusNone_ShouldThrowException()
        {
            // Arrange
            var emailService = new HealthCareABApi.Services.Implementations.EmailService();

            var appointment = new Appointment
            {
                Id = "",
                PatientId = "",
                CaregiverId = "",
                DateTime = new DateTime(),
                Status = AppointmentStatus.None
            };

            // Act
            var result = () => emailService.SendAppointmentEmail(appointment, false);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task SendAppointmentEmail_UnsupportedAppointmentStatus_ShouldThrowException()
        {
            // Arrange
            var emailService = new HealthCareABApi.Services.Implementations.EmailService();

            var appointment = new Appointment
            {
                Id = "",
                PatientId = "",
                CaregiverId = "",
                DateTime = new DateTime(),
                Status = (AppointmentStatus)9001 // this enum does not exist in the model
            };

            // Act
            var result = () => emailService.SendAppointmentEmail(appointment, false);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(result);
        }
    }
}
