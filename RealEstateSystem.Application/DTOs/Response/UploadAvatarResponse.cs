namespace RealEstateSystem.Application.DTOs.Response
{
    public class UploadAvatarResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
