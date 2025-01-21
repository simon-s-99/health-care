using Email.Net;

namespace HealthCareABApi.Services.Interfaces
{
    public interface IEmailService
    {
        EmailMessage ComposeEmail(string sendTo, string emailSubject, string emailMessage);
        Task<IResult> SendAppointmentEmail(string userId, string emailMessage);
        Task<IResult> SendConfirmedAppointmentEmail(string userId, string emailMessage);
        Task<IResult> SendChangedAppointmentEmail(string userId, string emailMessage);
        Task<IResult> SendCanceledAppointmentEmail(string userId, string emailMessage);
    }
}
