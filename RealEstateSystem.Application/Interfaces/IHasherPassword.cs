namespace RealEstateSystem.Application.Interfaces
{
    public interface IHasherPassword
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
