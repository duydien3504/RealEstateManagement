namespace RealEstateSystem.Domain.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message) : base(message, 401)
        {
        }
    }
}
