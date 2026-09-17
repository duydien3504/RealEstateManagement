using System;

namespace RealEstateSystem.Application.DTOs.Request
{
    public class PropertyMediaRequest
    {
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
    }
}
