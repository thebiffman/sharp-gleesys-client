using Microsoft.Extensions.Configuration;
using PublicIpUpdater;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var settings = config.GetRequiredSection("PublicIpUpdaterSettings");

var url 
    = Environment.GetEnvironmentVariable("WEBSERVICE_URL", EnvironmentVariableTarget.Process) 
      ?? settings["WebServiceUrl"];

var username 
    = Environment.GetEnvironmentVariable("GLESYS_USERNAME", EnvironmentVariableTarget.Process) 
      ?? settings["Username"];

var apiKey 
    = Environment.GetEnvironmentVariable("GLESYS_APIKEY", EnvironmentVariableTarget.Process) 
      ?? settings["ApiKey"];

var usePublicIpString 
    = Environment.GetEnvironmentVariable("GLESYS_USE_PUBLIC_IP", EnvironmentVariableTarget.Process) 
      ?? settings["UsePublicIp"];

var ipStartsWith 
    = Environment.GetEnvironmentVariable("GLESYS_IP_STARTS_WITH", EnvironmentVariableTarget.Process) 
      ?? settings["IpStartsWith"];

var ttlString 
    = Environment.GetEnvironmentVariable("GLESYS_TTL", EnvironmentVariableTarget.Process) 
      ?? settings["ttl"];

var usePublicIp = Convert.ToBoolean(usePublicIpString);
var ttl = Convert.ToInt32(ttlString);

var domains 
    = Environment.GetEnvironmentVariable("GLESYS_DOMAINS", EnvironmentVariableTarget.Process)
    ?? settings["Domains"];

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



