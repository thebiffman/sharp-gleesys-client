using Microsoft.Extensions.Configuration;
using PublicIpUpdater;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var url = configuration["GLESYS_WEBSERVICE_URL"] ?? "https://api.glesys.com";

var username = configuration["GLESYS_USERNAME"];

var apiKey = configuration["GLESYS_APIKEY"];

var usePublicIpString = configuration["GLESYS_USE_PUBLIC_IP"] ?? "true";

var ipStartsWith = configuration["GLESYS_IP_STARTS_WITH"];

var ttlString = configuration["GLESYS_TTL"] ?? "300";

var usePublicIp = Convert.ToBoolean(usePublicIpString);
var ttl = Convert.ToInt32(ttlString);

var domains = configuration["GLESYS_DOMAINS"];

if(string.IsNullOrEmpty(url))
{
    Console.WriteLine("Variable GLESYS_DOMAINS is required");
    return;
}

if (string.IsNullOrEmpty(username))
{
    Console.WriteLine("Variable GLESYS_USERNAME is required");
    return;
}

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("Variable GLESYS_APIKEY is required");
    return;
}

if (string.IsNullOrEmpty(domains))
{
    Console.WriteLine("Variable GLESYS_DOMAINS is required in format '<domain1>#<host1>,<host2>|<domain2>#<host1>,<host2>'");
    return;
}


if (!usePublicIp && string.IsNullOrEmpty(ipStartsWith))
{
    Console.WriteLine("Variable GLESYS_IP_STARTS_WITH is required when GLESYS_USE_PUBLIC_IP is set to false");
    return;
}

var recordList = new List<DomainRecord>();
foreach (var domainInfo in domains.Split("|"))
{
    var domainInfoSplit = domainInfo.Split("#");
    var domain = domainInfoSplit[0];
    var hosts = domainInfoSplit[1].Split(",");

    foreach (var host in hosts)
    {
        recordList.Add(new DomainRecord
        {
            Domain = domain,
            Host = host
        });
    }
}

var config = new PublicIpUpdaterSettings()
{
    Ttl = ttl,
    ApiKey = apiKey,
    Url = url,
    UsePublicIp = usePublicIp,
    UserName = username,
    IpStartsWith = ipStartsWith,
    RecordsToUpdate = recordList,
};

Console.WriteLine("Starting...");


var updateRecords = new UpdateRecords();
updateRecords.DoUpdate(config);

Console.WriteLine("Exiting...");
