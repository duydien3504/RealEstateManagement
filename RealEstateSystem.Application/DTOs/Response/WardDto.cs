using System;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class WardDto
    {
        public Guid WardId { get; set; }
        public Guid ProvinceId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
