using RealEstateSystem.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateSystem.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid RefreshTkId { get; private set; }
        public Guid UserId { get; private set; }
        public string Token {  get; private set; }
        public DateTime? Expired { get; private set; }
        public bool IsExpired { get; private set; }
        public DateTime? Revoked { get; private set; }
        [NotMapped]
        public bool IsActive => Revoked == null && !IsExpired;

        protected RefreshToken()
        {
        }

        public RefreshToken(Guid userId, string token, DateTime expires)
        {
            RefreshTkId = Guid.NewGuid();
            UserId = userId;
            Token = token;
            Expired = expires;
        }

        public void Revoke()
        {
            Revoked = DateTime.UtcNow;
        }
    }
}
