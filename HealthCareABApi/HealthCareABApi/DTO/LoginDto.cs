﻿using System.ComponentModel.DataAnnotations;
namespace HealthCareABApi.DTO
{
    public class LoginDTO
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}