using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.UnitTests.Common
{
    public static class TestDataBuilder
    {
        public static User CreateActiveUser(Guid userId, string email, string fullName, string passwordHash, RoleType roleType = RoleType.User)
        {
            return new User
            {
                UserId = userId,
                Email = email,
                FullName = fullName,
                PasswordHash = passwordHash,
                Status = StatusType.Active,
                IsDeleted = false,
                Role = new Role
                {
                    RoleId = Guid.NewGuid(),
                    NameRole = roleType
                }
            };
        }

        public static User CreateInactiveUser(Guid userId, string email)
        {
            return new User
            {
                UserId = userId,
                Email = email,
                FullName = "Inactive User",
                PasswordHash = "hashed",
                Status = StatusType.Inactive,
                IsDeleted = false,
                Role = new Role
                {
                    RoleId = Guid.NewGuid(),
                    NameRole = RoleType.User
                }
            };
        }

        public static User CreateBlockedUser(Guid userId, string email)
        {
            return new User
            {
                UserId = userId,
                Email = email,
                FullName = "Blocked User",
                PasswordHash = "hashed",
                Status = StatusType.Block,
                IsDeleted = false,
                Role = new Role
                {
                    RoleId = Guid.NewGuid(),
                    NameRole = RoleType.User
                }
            };
        }
    }
}
