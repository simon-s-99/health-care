namespace HealthCareABApi.DTO
{
    public class LoginResponseDTO
    {
        public string Message { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public string UserId { get; set; }
    }
}
