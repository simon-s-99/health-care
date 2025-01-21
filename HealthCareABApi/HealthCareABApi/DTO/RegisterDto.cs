using System;
using System.ComponentModel.DataAnnotations;
namespace HealthCareABApi.DTO
{
    public class RegisterDto
    {
        public required string Firstname { get; set; }

        public required string Lastname { get; set; }

        public required string Email { get; set; }

        public required string Phonenumber { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; }

        public required List<string> Roles { get; set; }
    }
}