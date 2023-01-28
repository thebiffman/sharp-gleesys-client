using System.Text.Json.Serialization;

namespace SharpGlesysClient.Dto.Domain;

public class Record
{
    [JsonPropertyName("recordid")]
    public int Recordid { get; set; }

    [JsonPropertyName("domainname")]
    public string Domainname { get; set; }

    [JsonPropertyName("host")]
    public string Host { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonPropertyName("ttl")]
    public int Ttl { get; set; }
}