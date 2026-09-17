using System;
using System.Collections.Generic;

namespace RealEstateSystem.Application.DTOs.Request
{
    public class UpdatePropertyRequest
    {
        public Guid CategoryId { get; set; }
        public Guid WardId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string? Dimensions { get; set; }
        public string? AddressDetail { get; set; }
        public int NumBedrooms { get; set; }
        public List<Guid> AmenityIds { get; set; } = new List<Guid>();
        public List<PropertyMediaRequest> Medias { get; set; } = new List<PropertyMediaRequest>();
    }
}
