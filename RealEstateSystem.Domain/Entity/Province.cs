namespace RealEstateSystem.Domain.Entity
{
    public class Province
    {
        public Guid ProvinceId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}
