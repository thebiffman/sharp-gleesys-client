using System.Text.Json.Serialization;

namespace SharpGlesysClient.Dto.Domain;

public class Status
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}