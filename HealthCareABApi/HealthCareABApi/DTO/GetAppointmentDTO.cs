using HealthCareABApi.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthCareABApi.DTO
{
    public class GetAppointmentDTO
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        public string CaregiverId { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.None;
    }
}
