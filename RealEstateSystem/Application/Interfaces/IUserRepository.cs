using RealEstateSystem.Domain.Entities;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
    }
}
