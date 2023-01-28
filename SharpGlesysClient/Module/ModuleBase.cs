namespace SharpGlesysClient.Module
{
    public class ModuleBase
    {
        internal readonly GlesysClient Client;

        public ModuleBase(GlesysClient client)
        {
            Client = client;
        }
    }
}
