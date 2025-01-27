namespace HealthCareABApi.DTO
{
    public class RegisterResponseDTO
    {
        public string Message { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}
