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
            string emailMessage = "";

            return await SendEmail(appointment, emailSubject, emailMessage);
        }

        public async Task<EmailSendingResult> SendChangedAppointmentEmail(Appointment appointment)
        {
            // TODO: implement sending of e-mails when an appointment is scheduled but
            //          the date or doctor (or any other details) have been modified.

            string emailSubject = "Your appointment has been changed!";
            string emailMessage = "";

            return await SendEmail(appointment, emailSubject, emailMessage);
        }

        public async Task<EmailSendingResult> SendCanceledAppointmentEmail(Appointment appointment)
        {
            string emailSubject = "Your appointment has been canceled.";
            string emailMessage = "";

            return await SendEmail(appointment, emailSubject, emailMessage);
        }

        public Task<EmailSendingResult> SendAppointmentEmail(Appointment appointment)
        {
            if (appointment.Status == AppointmentStatus.Scheduled)
            {
                return SendConfirmedAppointmentEmail(appointment);

                // TODO: implement sending of e-mails when an appointment is scheduled but
                //          the date or doctor (or any other details) have been modified.
                // return SendChangedAppointmentEmail(appointment);
            }
            else if (appointment.Status == AppointmentStatus.Cancelled)
            {
                return SendCanceledAppointmentEmail(appointment);
            }
            else
            {   // if appointment status == 'None'
                throw new ArgumentException("Appointment with invalid AppointmentStatus passed.", nameof(appointment));
            }
        }
    }
}
