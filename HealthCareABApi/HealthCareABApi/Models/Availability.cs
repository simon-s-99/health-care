using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class Availability
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        // Reference to Caregiver (User)
        [BsonRepresentation(BsonType.ObjectId)]
        public required string CaregiverId { get; set; }

        public List<DateTime>? AvailableSlots { get; set; }
    }
}

