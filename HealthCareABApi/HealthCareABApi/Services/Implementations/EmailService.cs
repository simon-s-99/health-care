/*
    As a general comment about this Service:
        We could have used System.Net.Mail class for all of this & reduced at least 1 
        dependency for this project but Email.Net was just more convenient.
 */

using Email.Net;
using HealthCareABApi.Services.Interfaces;

namespace HealthCareABApi.Services.Implementations
{
    public class EmailService : Interfaces.IEmailService
    {
        // DI for .AddEmailNet service
        private readonly Email.Net.IEmailService _emailService;
        private readonly UserService _userService;

        public EmailService(Email.Net.IEmailService emailService, UserService userService)
        {
            _emailService = emailService;
            _userService = userService;
        }

        /// <summary>
        /// Composes an EmailMessage instance.
        /// </summary>
        /// <param name="sendTo">Receivers e-mail adress</param>
        /// <param name="emailSubject">The subject text of the e-mail</param>
        /// <param name="emailMessage">The main plain-text content of the e-mail</param>
        /// <returns>An instance of EmailMessage, ready to be sent.</returns>
        public EmailMessage ComposeEmail(string sendTo, string emailSubject, string emailMessage)
        {
            var message = EmailMessage.Compose()
                .SetCharsetTo("utf-8")
                .To(sendTo)
                .WithSubject(emailSubject)
                .WithPlainTextContent(emailMessage)
                .WithNormalPriority()
                .Build();

            return message;
        }

        public async Task<IResult> SendAppointmentEmail(string userId, string emailMessage)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            throw new NotImplementedException();
        }

        public async Task<IResult> SendConfirmedAppointmentEmail(string userId, string emailMessage)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> SendChangedAppointmentEmail(string userId, string emailMessage)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> SendCanceledAppointmentEmail(string userId, string emailMessage)
        {
            throw new NotImplementedException();
        }
    }
}
