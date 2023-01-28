using Microsoft.Extensions.Configuration;
using SharpGlesysClient;
using SharpGlesysClient.Dto.Domain;

Console.WriteLine("Starting...");

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settings = config.GetRequiredSection("Settings");

var url = settings["WebServiceUrl"];
var username = settings["Username"];
var apiKey = settings["ApiKey"];

if (url == null || username == null || apiKey == null)
{
    Console.WriteLine("Missing configuration, exiting...");
    Console.ReadKey();
    return;
}

Console.WriteLine();

var glesysClient = new GlesysClient(url, username, apiKey);

var records = glesysClient.Domain.ListRecords("ason.nu");

foreach (var record in records.Result.Response.Records)
{
    Console.WriteLine($"{record.Recordid}-{record.Host}.{record.Domainname}:{record.Data} ({record.Ttl})");
}

Console.WriteLine();

var selectedRecord = records.Result.Response.Records
    .FirstOrDefault(x => 
        x.Type.Equals("TXT", StringComparison.CurrentCultureIgnoreCase) && 
        x.Host.Equals("hello", StringComparison.InvariantCultureIgnoreCase)
        );

if (selectedRecord == null)
{
    Console.WriteLine("Unable to find the desired record, exiting...");
    Console.ReadKey();
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
    Data = selectedRecord.Data + "7",
    //Ttl = selectedRecord.Ttl.ToString(),
    //Type = selectedRecord.Type,
});
Console.WriteLine($"Request status: {updateResult.Result.Response.Status.Code}");
Console.WriteLine("Returned record:");
Console.WriteLine($"{updateResult.Result.Response.Record.Recordid}-{updateResult.Result.Response.Record.Host}.{updateResult.Result.Response.Record.Domainname}:{updateResult.Result.Response.Record.Data} ({updateResult.Result.Response.Record.Ttl})");

Console.ReadKey();
