using SharpGlesysClient;
using SharpGlesysClient.Dto.Domain;
using System.Net;

namespace PublicIpUpdater
{
    internal class UpdateRecordsWIthPublicIp
    {
        public UpdateRecordsWIthPublicIp()
        {
            
        }

        public void Run(PublicIpUpdaterSettings config)
        {
            Console.WriteLine();

            var ipAddress = GetPublicIpAddress();

            if (ipAddress == null)
            {
                Console.WriteLine("Unable to find relevant IP address, skipping update.");
                return;
            }

            var recordsDict = config.RecordsToUpdate
                .GroupBy(x => x.Domain)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.Host).ToList()
                    );

            Console.WriteLine();

            var glesysClient = new GlesysClient(config.Url, config.UserName, config.ApiKey);

            foreach (var domainToUpdate in recordsDict)
            {
                var currentRecords = glesysClient.Domain.ListRecords(domainToUpdate.Key).Result.Response.Records;

                Console.WriteLine("Printing all records for domain:");
                foreach (var record in currentRecords)
                {
                    Console.WriteLine($"    {record.Recordid}-{record.Host}.{record.Domainname}:{record.Data} ({record.Ttl})");
                }

                foreach (var host in domainToUpdate.Value)
                {
                    var selectedRecord = currentRecords
                        .FirstOrDefault(x =>
                            x.Type.Equals("A", StringComparison.CurrentCultureIgnoreCase) &&
                            x.Host.Equals(host, StringComparison.InvariantCultureIgnoreCase)
                        );

                    if (selectedRecord == null) // TODO CREATE
                    {
                        Console.WriteLine("Unable to find the desired record, continue...");
                        return;
                    }

                    Console.WriteLine();
                    Console.WriteLine("Selected record to update:");
                    Console.WriteLine($"{selectedRecord.Recordid}-{selectedRecord.Host}.{selectedRecord.Domainname}:{selectedRecord.Data} ({selectedRecord.Ttl})");

                    if (selectedRecord.Ttl == config.Ttl &&
                        selectedRecord.Data.Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine("Existing values are correct, no update needed.");
                        continue;
                    }

                    Console.WriteLine();
                    Console.WriteLine("Updating selected record with new values...");
                    var updateResult = glesysClient.Domain.UpdateRecord(new UpdateRecordRequest
                    {
                        Recordid = selectedRecord.Recordid.ToString(),
                        //Host = selectedRecord.Host,
                        Data = ipAddress,
                        Ttl = config.Ttl > 0 ? config.Ttl.ToString() : selectedRecord.Ttl.ToString(),
                        //Type = selectedRecord.Type,
                    });
                    Console.WriteLine($"Request status: {updateResult.Result.Response.Status.Code}");
                    Console.WriteLine("Returned record:");
                    Console.WriteLine($"{updateResult.Result.Response.Record.Recordid}-{updateResult.Result.Response.Record.Host}.{updateResult.Result.Response.Record.Domainname}:{updateResult.Result.Response.Record.Data} ({updateResult.Result.Response.Record.Ttl})");
                    Console.WriteLine();
                }


            }
        }

        private string? GetPublicIpAddress()
        {
            //https://api.ipify.org

            Console.Write(Environment.NewLine + "Finding public ip...");

            var client = new HttpClient();
            var ip = client.GetStringAsync(new Uri("https://api.ipify.org")).Result;
            var externalIp = IPAddress.Parse(ip);

            var ipString = externalIp.ToString();

            Console.Write(" Done. IP: " + ipString + Environment.NewLine);

            return ipString;
        }
    }
}
