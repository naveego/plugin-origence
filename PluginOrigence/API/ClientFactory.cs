using PluginOrigence.Helper;

public class ClientFactory : IClientFactory
{
    public IPluginClient GetClient(Settings settings)
    {
        if (settings.UseRemote)
        {
            if (settings.UseSftp)
            {
                return new OrigenceSftpClient(settings);
            }
            else
            {
                return new OrigenceFtpClient(settings);
            }
        }
        else
        {
            return new OrigenceLfsClient(settings);
        }
    }
}