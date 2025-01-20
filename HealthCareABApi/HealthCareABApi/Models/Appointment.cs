using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class Appointment
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        // Reference to Patient (User)
        [BsonRepresentation(BsonType.ObjectId)]
        public required string PatientId { get; set; }

        // Reference to Caregiver (User)
        [BsonRepresentation(BsonType.ObjectId)]
        public required string CaregiverId { get; set; }

        public DateTime DateTime { get; set; }

        public AppointmentStatus Status { get; set; }
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        None
    }
}

