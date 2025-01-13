using HealthCareABApi.Models;

namespace HealthCareABApi.DTO
{
    public class UpdateAppointmentDTO
    {
        public DateTime DateTime { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}

