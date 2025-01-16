using System.ComponentModel.DataAnnotations;
namespace HealthCareABApi.DTO
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}