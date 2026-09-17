using System;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class PropertyMediaResponseDto
    {
        public Guid MediaId { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
    }
}
