using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthCareABApi.Models
{
    public class User
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        // MongoDB ObjectId stored as a string
        public string Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [BsonRequired]
        public required string Firstname { get; set; }
        [BsonRequired]
        public required string Lastname { get; set; }
        [BsonRequired]
        public required string Email { get; set; }
        [BsonRequired]
        public required string Phonenumber { get; set; }
        [BsonRequired]
        public required string Username { get; set; }
        [BsonRequired]
        public required string PasswordHash { get; set; }
        // List of roles, a User can have one or more roles if needed.
        // Not specifying a role during User creation sets it to User by default
        public required List<string> Roles { get; set; }
    }
}
