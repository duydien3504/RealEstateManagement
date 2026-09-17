using RealEstateSystem.Domain.Common;

namespace RealEstateSystem.Domain.Entity
{
    public class Category : AuditableEntity
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
