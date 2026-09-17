namespace RealEstateSystem.Application.DTOs.Request
{
    public class UpRoleRequest
    {
        public string Email { get; set; } = string.Empty;
        public string IdCardNumber { get; set; } = string.Empty;
        public string? RawDocumentsUrl { get; set; }
    }
}
