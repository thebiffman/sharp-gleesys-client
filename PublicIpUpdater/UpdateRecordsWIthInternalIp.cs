using SharpGlesysClient;
using SharpGlesysClient.Dto.Domain;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace PublicIpUpdater
{
    internal class UpdateRecordsWIthInternalIp
    {
        public UpdateRecordsWIthInternalIp()
        {
            
        }

        public void Run(UpdateWithInternalIpConfiguration config)
        {
            Console.WriteLine();

            var ipAddress = GetRelevantIpAddress(config.IpStartsWith);

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

            var glesysClient = new GlesysClient(config.Url, config.UserName, config.ApiKey);

            foreach (var domainToUpdate in recordsDict)
            {
                var currentRecords = glesysClient.Domain.ListRecords(domainToUpdate.Key).Result.Response.Records;

                foreach (var record in currentRecords)
                {
                    Console.WriteLine($"{record.Recordid}-{record.Host}.{record.Domainname}:{record.Data} ({record.Ttl})");
                }

                Console.WriteLine();

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
                    Console.WriteLine("Selected record:");
                    Console.WriteLine($"{selectedRecord.Recordid}-{selectedRecord.Host}.{selectedRecord.Domainname}:{selectedRecord.Data} ({selectedRecord.Ttl})");

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
                }

                
            }

        }

        private string? GetRelevantIpAddress(string ipStartsWith)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in interfaces)
            {
                var ipProps = adapter.GetIPProperties();

                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up
                        && ip.Address.AddressFamily == AddressFamily.InterNetwork
                        && ip.Address.ToString().StartsWith(ipStartsWith, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Console.WriteLine($"Found relevant ip address '{ip.Address}' on adapter '{adapter.Description}'");
                        return ip.Address.ToString();
                    }
                }
            }

            return null;
        }
    }
}
