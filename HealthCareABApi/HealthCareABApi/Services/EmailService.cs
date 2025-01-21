/*
    As a general comment about this Service:
        We could have used System.Net.Mail class for all of this & reduced at least 1 
        dependency for this project but Email.Net was just more convenient.
 */

using Email.Net;

namespace HealthCareABApi.Services
{
    public class EmailService
    {
        // DI for .AddEmailNet service
        private readonly IEmailService _emailService;
        private readonly UserService _userService;

        public EmailService(IEmailService emailService, UserService userService)
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
        private EmailMessage ComposeEmail(string sendTo, string emailSubject, string emailMessage)
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

        public async void SendBookedAppointmentEmail(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            //EmailMessage emailMessage = ComposeEmail(user.Email);
        }
    }
}
