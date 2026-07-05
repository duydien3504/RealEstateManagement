namespace RealEstateSystem.Domain.Entity
{
    public class Ward
    {
        public Guid WardId { get; set; }
        public Guid ProvinceId { get; set; }
        public string Name { get; set; } = string.Empty;

        public Province Province { get; set; } = null!;

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
