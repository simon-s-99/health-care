using Email.Net;
using HealthCareABApi.Models;

namespace HealthCareABApi.Services.Interfaces
{
    public interface IEmailService
    {
        EmailMessage ComposeEmail(string sendTo, string emailSubject, string emailMessage);
        Task<EmailSendingResult> SendEmail(Appointment appointment, string emailSubject, string emailMessage);
        Task<EmailSendingResult> SendConfirmedAppointmentEmail(Appointment appointment);
        Task<EmailSendingResult> SendChangedAppointmentEmail(Appointment appointment);
        Task<EmailSendingResult> SendCanceledAppointmentEmail(Appointment appointment);
        Task<EmailSendingResult> SendAppointmentEmail(Appointment appointment);
    }
}
