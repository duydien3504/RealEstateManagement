namespace RealEstateSystem.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string HasHPassword(string Password);
        bool VerifyPassword(string Password, string HashedPassword);
    }
}
