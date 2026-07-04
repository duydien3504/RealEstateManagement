using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Infrastructure.Security
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string HasHPassword(string Password)
        {
            return BCrypt.Net.BCrypt.HashPassword(Password, workFactor: 11);
        }

        public bool VerifyPassword(string Password, string HashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(Password, HashedPassword);
        }
    }
}
