using System.Text.Json.Serialization;

namespace SharpGlesysClient.Dto.Domain
{
    public class ListRecordsResult
    {
        [JsonPropertyName("response")]
        public ListRecordsResponse Response { get; set; }

        public class ListRecordsResponse
        {
            [JsonPropertyName("status")]
            public Status Status { get; set; }

            [JsonPropertyName("records")]
            public Record[] Records { get; set; }
        }
    }
}
