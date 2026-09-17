using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class ProvinceSyncDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("wards")]
        public List<WardSyncDto> Wards { get; set; } = new List<WardSyncDto>();
    }
}
