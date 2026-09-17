using System;
using System.Collections.Generic;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class PropertyResponseDto
    {
        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid WardId { get; set; }
        public string WardName { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string? Dimensions { get; set; }
        public string? AddressDetail { get; set; }
        public int NumBedrooms { get; set; }
        public string PropertyStatus { get; set; } = string.Empty;
        public string DisplayStatus { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PropertyMediaResponseDto> Medias { get; set; } = new List<PropertyMediaResponseDto>();
        public List<AmenityResponseDto> Amenities { get; set; } = new List<AmenityResponseDto>();
    }
}
