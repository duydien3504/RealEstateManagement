namespace RealEstateSystem.Domain.Exceptions
{
    public class BadRequestException : DomainException
    {
        public BadRequestException(string message) : base(message, 400)
        {
        }
    }
}
