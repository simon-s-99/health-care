using System.ComponentModel.DataAnnotations;

namespace HealthCareABApi.DTO
{
    public class UpdateDTO
    {
        // All fields are nullable (?) to allow partial updates.
        // If a field is null in the request, it will not be updated.
        public string? Firstname { get; set; }
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phonenumber { get; set; }

        [StringLength(25, ErrorMessage = "Username cannot exceed 25 characters")]
        public string? Username { get; set; }

    }
}
