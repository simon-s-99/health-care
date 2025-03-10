﻿using System.ComponentModel.DataAnnotations;

namespace HealthCareABApi.DTO
{
    public class GetUserDTO
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phonenumber { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public List<string> Roles { get; set; }
    }
}
