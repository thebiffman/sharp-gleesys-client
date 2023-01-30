using SharpGlesysClient;
using SharpGlesysClient.Dto.Domain;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PublicIpUpdater
{
    internal class UpdateRecords
    {
        public void DoUpdate(PublicIpUpdaterSettings config)
        {
            Console.WriteLine();

            var ipAddress = GetRelevantIpAddress(config);

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

                Console.WriteLine();
                Console.WriteLine($"Printing all A records for domain '{domainToUpdate.Key}':");
                foreach (var record in currentRecords.Where(x => x.Type.Equals("a", StringComparison.InvariantCultureIgnoreCase)))
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
                        Console.WriteLine();
                        Console.WriteLine($"Unable to find the desired A record with host '{host}', continue...");
                        continue;
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

        private string? GetRelevantIpAddress(PublicIpUpdaterSettings config)
        {
            if (config.UsePublicIp)
            {
                Console.Write(Environment.NewLine + "Finding public ip...");

                var client = new HttpClient();
                var ip = client.GetStringAsync(new Uri("https://api.ipify.org")).Result;
                var externalIp = IPAddress.Parse(ip);

                var ipString = externalIp.ToString();

                Console.Write(" Done. IP: " + ipString + Environment.NewLine);

                return ipString;
            }
            else
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var adapter in interfaces)
                {
                    var ipProps = adapter.GetIPProperties();

                    foreach (var ip in ipProps.UnicastAddresses)
                    {
                        if (adapter.OperationalStatus == OperationalStatus.Up
                            && ip.Address.AddressFamily == AddressFamily.InterNetwork
                            && ip.Address.ToString().StartsWith(config.IpStartsWith, StringComparison.CurrentCultureIgnoreCase))
                        {
                            Console.WriteLine($"Found relevant ip address '{ip.Address}' on adapter '{adapter.Description}'");
                            return ip.Address.ToString();
                        }
                    }
                }
            }

            return null;
        }
    }
}
