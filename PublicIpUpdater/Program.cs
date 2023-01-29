using Microsoft.Extensions.Configuration;
using PublicIpUpdater;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settings = config.GetRequiredSection("PublicIpUpdaterSettings");

var url = settings["WebServiceUrl"];
var username = settings["Username"];
var apiKey = settings["ApiKey"];
var usePublicIp = Convert.ToBoolean(settings["UsePublicIp"]);
var ipStartsWith = settings["IpStartsWith"];
var ttl = Convert.ToInt32(settings["ttl"]);

var domainsSection = settings.GetSection("Domains");

var recordList = new List<DomainRecord>();
foreach (var record in domainsSection.GetChildren())
{
    if (string.IsNullOrEmpty(record.Key))
    {
        continue;
    }

    foreach (var hosts in record.GetChildren())
    {
        if (string.IsNullOrEmpty(hosts.Value) || string.IsNullOrEmpty(hosts.Key))
        {
            continue;
        }
        recordList.Add(new DomainRecord
        {
            Domain = record.Key,
            Host = hosts.Value
        });
    }
}

Console.WriteLine("Starting...");

if (url == null || username == null || apiKey == null || (usePublicIp == false && string.IsNullOrEmpty(ipStartsWith)))
{
    Console.WriteLine("Missing configuration, exiting...");
    Console.ReadKey();
    return;
}

if (usePublicIp)
{
    var updateRecordsWithPublicIp = new UpdateRecordsWIthPublicIp();
    updateRecordsWithPublicIp.Run(new PublicIpUpdaterSettings
    {
        Ttl = ttl,
        ApiKey = apiKey,
        Url = url,
        UserName = username,
        RecordsToUpdate = recordList,
    });
    return;
}

if (!usePublicIp && !string.IsNullOrEmpty(ipStartsWith))
{
    var updateRecordsWithInternalIp = new UpdateRecordsWIthInternalIp();
    updateRecordsWithInternalIp.Run(new UpdateWithInternalIpConfiguration
    {
        Ttl = ttl,
        ApiKey = apiKey,
        Url = url,
        UserName = username,
        IpStartsWith = ipStartsWith,
        RecordsToUpdate = recordList,
    });
}



