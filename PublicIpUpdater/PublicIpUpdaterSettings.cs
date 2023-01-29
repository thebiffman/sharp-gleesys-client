namespace PublicIpUpdater
{
    internal class PublicIpUpdaterSettings
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string ApiKey { get; set; }
        public int Ttl { get; set; }
        public List<DomainRecord> RecordsToUpdate { get; set; }
    }

    internal class UpdateWithInternalIpConfiguration : PublicIpUpdaterSettings
    {
        public string IpStartsWith { get; set; }
    }

    internal class DomainRecord
    {
        public string Host { get; set; }
        public string Domain { get; set; }
    }
}
