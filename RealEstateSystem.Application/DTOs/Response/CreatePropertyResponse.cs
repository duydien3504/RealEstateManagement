using System;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class CreatePropertyResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? PropertyId { get; set; }
    }
}
