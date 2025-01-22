/*
    As a general comment about this Service:
        We could have used System.Net.Mail class for all of this & reduced at least 1 
        dependency for this project but Email.Net was just more convenient.
 */

using Email.Net;
using HealthCareABApi.Models;
using HealthCareABApi.Services.Interfaces;

namespace HealthCareABApi.Services.Implementations
{
    public class EmailService
    {
        // DI for .AddEmailNet service
        private readonly Email.Net.IEmailService _emailService;
        private readonly UserService _userService;

        public EmailService(Email.Net.IEmailService emailService, UserService userService)
        {
            _emailService = emailService;
            _userService = userService;
        }

        public EmailService() { } // empty constructor mainly used for tests

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
                .To(new System.Net.Mail.MailAddress(sendTo))
                .WithSubject(emailSubject)
                .WithPlainTextContent(emailMessage)
                .WithNormalPriority()
                .Build();

            return message;
        }

        /// <summary>
        /// Attempt to send an e-mail via the Dependency Injected Email.Net.EmailService
        /// </summary>
        /// <param name="appointment">An instance of Appointment model.</param>
        /// <param name="emailSubject">The subject text of the e-mail</param>
        /// <param name="emailMessage">The main plain-text content of the e-mail</param>
        /// <returns>The EmailSendingResult for the e-mail.</returns>
        /// <exception cref="KeyNotFoundException">If user is not found in Database throw KeyNotFoundException.</exception>
        public async Task<EmailSendingResult> SendEmail(Appointment appointment, string emailSubject, string emailMessage)
        {
            var user = await _userService.GetUserByIdAsync(appointment.PatientId);

            if (user is not null)
            {
                string recipientEmail = user.Email;
                EmailMessage emailToBeSent = ComposeEmail(recipientEmail, emailSubject, emailMessage);

                return await _emailService.SendAsync(emailToBeSent);
            }
            else
            {   // If user is null throw exception, KeyNotFound was the best i could find
                throw new KeyNotFoundException("User not found in DB, check userId.");
            }
        }

        public async Task<EmailSendingResult> SendConfirmedAppointmentEmail(Appointment appointment)
        {
            string emailSubject = "Your appointment has been confirmed!";
            string emailMessage = $"Your appointment at {appointment.DateTime.ToString()} has been confirmed.";

            return await SendEmail(appointment, emailSubject, emailMessage);
        }

        public async Task<EmailSendingResult> SendUpdatedAppointmentEmail(Appointment appointment)
        {
            string emailSubject = "Your appointment has been updated!";
            string emailMessage = $"Your appointment time has been changed to {appointment.DateTime.ToString()}.";

            return await SendEmail(appointment, emailSubject, emailMessage);
        }

        public async Task<EmailSendingResult> SendCanceledAppointmentEmail(Appointment appointment)
        {
            string emailSubject = "Your appointment has been canceled.";
            string emailMessage = $"Your appointment at {appointment.DateTime.ToString()} has been canceled.";

            return await SendEmail(appointment, emailSubject, emailMessage);
        }

        /// <summary>
        /// Send an e-mail based on the status of the Appointment instance.
        /// </summary>
        /// <param name="appointment">An instance of Appointment model.</param>
        /// <param name="updatedExistingAppointment">Whether or not the appointment passed is an existing appointment that has been updated.</param>
        /// <returns>The EmailSendingResult for the e-mail.</returns>
        /// <exception cref="ArgumentException">If an appointment with status 'None' is passed throw ArgumentException.</exception>
        /// <exception cref="InvalidOperationException">If an appointment with invalid AppointmentStatus is passed throw InvalidOperationException.</exception>
        public async Task<EmailSendingResult> SendAppointmentEmail(
            Appointment appointment,
            bool updatedExistingAppointment = false)
        {
            if (updatedExistingAppointment)
            {
                return await SendUpdatedAppointmentEmail(appointment);
            }
            else if (appointment.Status == AppointmentStatus.Scheduled)
            {
                return await SendConfirmedAppointmentEmail(appointment);
            }
            else if (appointment.Status == AppointmentStatus.Cancelled)
            {
                return await SendCanceledAppointmentEmail(appointment);
            }
            else if (appointment.Status == AppointmentStatus.None)
            {
                throw new ArgumentException("Appointment with AppointmentStatus.None passed.", nameof(appointment.Status));
            }
            else
            {
                throw new InvalidOperationException("Appointment with invalid AppointmentStatus passed.");
            }
        }
    }
}
