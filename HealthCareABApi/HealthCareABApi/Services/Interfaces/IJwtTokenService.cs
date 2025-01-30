using HealthCareABApi.Models;

namespace HealthCareABApi.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}

