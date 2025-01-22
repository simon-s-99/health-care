using Xunit;
using HealthCareABApi.Models;
using System.Net.Mail;

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

        [Fact]
        public void ComposeEmailTest_EmailComposedCorrectly()
        {
            // Arrange
            var emailService = new HealthCareABApi.Services.Implementations.EmailService();

            var recipient = new MailAddress("test@example.com");
            string subject = "TestSubject";
            string plainTextBody = "TestBody";

            // Act
            var result = emailService.ComposeEmail(recipient.Address, subject, plainTextBody);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipient.Address, result.To.FirstOrDefault().Address);
            Assert.Equal(subject, result.Subject);
            Assert.Equal(plainTextBody, result.PlainTextBody);
        }
    }
}
