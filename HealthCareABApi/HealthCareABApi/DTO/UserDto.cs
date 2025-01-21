namespace HealthCareABApi.DTO
{
    public class UserDTO
    {
        public UserDTO(string username, List<string> roles)
        {
            Username = username;
            Roles = roles;
        }

        private string Username { get; set; }

        private List<string> Roles { get; set; }
    }
}
