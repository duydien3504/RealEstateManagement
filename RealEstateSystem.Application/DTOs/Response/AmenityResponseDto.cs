using System;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class AmenityResponseDto
    {
        public Guid AmenityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
    }
}
