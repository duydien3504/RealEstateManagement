using RealEstateSystem.Domain.Entities;

namespace RealEstateSystem.Application.Interfaces
{
    public interface ITokenProvider
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
