using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class Feedback
    {
      
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Reference to Appointment
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "Appointment ID is required.")]
        public string AppointmentId { get; set; }

        // Reference to Patient (User)
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "Patient ID is required.")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string Comment { get; set; }

    }
}

