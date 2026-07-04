using RealEstateSystem.Domain.Common;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entities
{
    public class OtpVerification : BaseEntity
    {
        public int OtpId { get; private set; }
        public Guid UserId { get; private set; }
        public int OtpCode { get; private set; }
        public OtpType Purpose { get; private set; }
        public DateTime ExpiredAt { get; private set; } 
        public bool IsUsed { get; private set; }
        
        protected OtpVerification() { }

        public OtpVerification (int _OtpId, Guid _UserId,  int _OtpCode, OtpType _Purpose, Boolean _IsUsed)
        {
            OtpId = _OtpId;
            UserId = _UserId;
            OtpCode = _OtpCode;
            Purpose = _Purpose;
            ExpiredAt = CreatedAt.AddMinutes(10);
            IsUsed = false;
        }

        public bool Verify(int InputOtp)
        {
            if (IsUsed)
                return false;
            if (DateTime.UtcNow > ExpiredAt)
                return false;
            if (OtpCode != InputOtp)
                return false;
            MarkAsUsed();
            return true;
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
        }
    }
}
