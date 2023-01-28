using System.Reflection.Metadata.Ecma335;
using System.Text.Encodings.Web;
using RestSharp;
using SharpGlesysClient.Dto.Domain;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp.Serializers.Json;

namespace SharpGlesysClient.Module
{
    public class Domain : ModuleBase
    {
        private const string Route = "domain";

        public Domain(GlesysClient client) : base(client)
        {
        }

        public async Task<ListRecordsResult> ListRecords(string domainName)
        {
            var request = new RestRequest($"{Route}/listrecords")
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(new { domainname = domainName });

            var response = await Client.RestClient.PostAsync<ListRecordsResult>(request);

            return await Task.FromResult(response ?? new ListRecordsResult());
        }

        public async Task<UpdateRecordResult?> UpdateRecord(UpdateRecordRequest updateRecordRequest)
        {
            var request = new RestRequest($"{Route}/updaterecord")
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json,
            };

            updateRecordRequest.Host = string.IsNullOrEmpty(updateRecordRequest.Host)
                ? null
                : updateRecordRequest.Host;

            updateRecordRequest.Data = string.IsNullOrEmpty(updateRecordRequest.Data)
                ? null
                : updateRecordRequest.Data;

            updateRecordRequest.Ttl = string.IsNullOrEmpty(updateRecordRequest.Ttl)
                ? null
                : updateRecordRequest.Ttl;

            updateRecordRequest.Type = string.IsNullOrEmpty(updateRecordRequest.Type)
                ? null
                : updateRecordRequest.Type;

            Client.RestClient.UseSystemTextJson(new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            });

            request.AddJsonBody(updateRecordRequest);

            var response = await Client.RestClient.PostAsync<UpdateRecordResult>(request);

            return await Task.FromResult(response ?? null);
        }
    }
}
