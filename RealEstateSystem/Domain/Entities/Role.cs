using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entities
{
    public class Role
    {
        public int RoleId { get; private set; }
        public RoleType RoleName { get; private set; }

        protected Role() { }
        public Role (int _roleId, RoleType _roleName)
        {
            RoleId = _roleId;
            RoleName = _roleName;
        }
    }
}
