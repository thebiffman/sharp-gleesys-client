using System.Text.Json.Serialization;

namespace SharpGlesysClient.Dto.Domain
{
    public class UpdateRecordResult
    {
        [JsonPropertyName("response")]
        public UpdateRecordResponse Response { get; set; }
    }

    public class UpdateRecordResponse
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("record")]
        public Record Record { get; set; }
    }

}
