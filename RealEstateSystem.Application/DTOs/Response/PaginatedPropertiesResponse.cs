using System.Collections.Generic;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class PaginatedPropertiesResponse
    {
        public List<PropertyResponseDto> Items { get; set; } = new List<PropertyResponseDto>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
