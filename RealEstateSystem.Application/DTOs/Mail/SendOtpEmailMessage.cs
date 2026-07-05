namespace RealEstateSystem.Application.DTOs.Mail
{
    public class SendOtpEmailMessage
    {
        public string ToEmail { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}
