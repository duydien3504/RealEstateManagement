namespace RealEstateSystem.Domain.Common
{
    public abstract class SoftDeleteEntity : AuditableEntity
    {
        public bool IsDeleted { get; set; }
    }
}
