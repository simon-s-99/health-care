using Xunit;
using Moq;
using Email.Net;
using HealthCareABApi.Models;

namespace HealthCareABApi.Tests
{
    public class EmailServiceTests
    {
        // Inject base versions of Mocks so that we do not have to re-create it for every test
        private readonly Mock<HealthCareABApi.Services.Implementations.EmailService> _emailServiceMock;
        //private readonly Mock<Email.Net.IEmailService> _fromPackageEmailServiceMock;
        //private readonly Mock<HealthCareABApi.Services.Implementations.UserService> _userServiceMock;

        public EmailServiceTests()
        {
            _emailServiceMock = new Mock<HealthCareABApi.Services.Implementations.EmailService>(
                //_fromPackageEmailServiceMock.Object,
                //_userServiceMock.Object
                );
        }

        [Fact]
        public async Task SendAppointmentEmail_UnsupportedAppointmentStatusNone_ShouldThrowException()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = "",
                PatientId = "",
                CaregiverId = "",
                DateTime = new DateTime(),
                Status = AppointmentStatus.Completed
            };

            //_emailServiceMock.Setup(service => service.SendAppointmentEmail(appointment, false)).Throws<Exception>();

            // Act
            var result = () => _emailServiceMock.Object.SendAppointmentEmail(appointment, false);

            Console.WriteLine( result.ToString() );

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }
    }
}
