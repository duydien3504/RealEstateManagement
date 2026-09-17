using System;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class ProvinceDto
    {
        public Guid ProvinceId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
