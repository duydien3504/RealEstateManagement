using System;

namespace RealEstateSystem.Application.DTOs.Request
{
    public class GetPublicPropertiesRequest
    {
        public string? Keyword { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? WardId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinArea { get; set; }
        public decimal? MaxArea { get; set; }
        public int? NumBedrooms { get; set; }
        public bool? IsPremium { get; set; }
        public string? SortBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
