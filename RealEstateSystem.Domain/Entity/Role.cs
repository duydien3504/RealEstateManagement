using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entity
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public RoleType NameRole { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
