using System.Text.Json.Serialization;

namespace SharpGlesysClient.Dto.Domain
{
    public class UpdateRecordRequest
    {
        [JsonPropertyName("recordid")]
        public string Recordid { get; set; }

        [JsonPropertyName("ttl")]
        public string? Ttl { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }
}
