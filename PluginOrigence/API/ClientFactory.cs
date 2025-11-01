using PluginOrigence.Helper;

public class ClientFactory : IClientFactory
{

    public IPluginClient GetClient(Settings settings)
    {
        var client = new OrigenceSftpClient(settings);
        return client;
    }
}