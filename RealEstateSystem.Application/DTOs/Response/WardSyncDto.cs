using System.Text.Json.Serialization;

namespace RealEstateSystem.Application.DTOs.Response
{
    public class WardSyncDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
