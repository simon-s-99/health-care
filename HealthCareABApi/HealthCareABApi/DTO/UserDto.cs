using System;
namespace HealthCareABApi.DTO
{
    public class UserDto
    {
        public UserDto(string username, List<string> roles)
        {
            Username = username;
            Roles = roles;
        }

        private string Username { get; set; }
        private List<string> Roles { get; set; }
    }
}
