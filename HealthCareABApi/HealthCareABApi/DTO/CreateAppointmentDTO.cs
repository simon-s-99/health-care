using HealthCareABApi.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HealthCareABApi.DTO
{
    public class CreateAppointmentDTO
    {
        public string PatientId { get; set; }

        public string CaregiverId { get; set; }

        public DateTime DateTime { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}