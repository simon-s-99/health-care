using HealthCareABApi.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthCareABApi.DTO
{
    public class CreateAppointmentDTO
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        public string CaregiverId { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; } = new DateTime(1111, 11, 11); // Initialize as an invalid date

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.None;
    }
}