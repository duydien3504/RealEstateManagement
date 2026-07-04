using RealEstateSystem.Domain.Common;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid UserId { get; private set; }
        public int RoleId { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public StatusAccount Status {  get; private set; }
        public string UrlAvatar { get; private set; } = string.Empty;
        public bool IsDeleted { get; private set; }
        public DateTime? LastLogin { get; private set; }

        public User (string fullName, string email, string passwordHash)
        {
            UserId = Guid.NewGuid();
            RoleId = (int)RoleType.User;
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Status = StatusAccount.Inactive;
            IsDeleted = false;
        }

        public void UpdateRole(int _RoleId)
        {
            if (_RoleId <= 0)
            {
                throw new ArgumentException("Role khong hop le");
            }
            RoleId = _RoleId;
        }

        public void UploadAvatar(string avatarUrl)
        {
            if (avatarUrl == null)
                throw new ArgumentNullException("Truong anh khong duoc rong");
            UrlAvatar = avatarUrl;
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

    }
}
