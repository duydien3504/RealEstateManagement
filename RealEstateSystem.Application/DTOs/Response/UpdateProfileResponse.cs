namespace RealEstateSystem.Application.DTOs.Response
{
    public class UpdateProfileResponse
    {
        public string Message { get; set; } = string.Empty;
        public UserProfileResponse UpdatedProfile { get; set; } = null!;
    }
}
