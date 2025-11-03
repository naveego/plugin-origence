using Naveego.Sdk.Logging;
using PluginOrigence.Helper;

public class ClientFactory : IClientFactory
{
    public IPluginClient GetClient(Settings settings)
    {
        if (settings.UseSftp)
        {
            Logger.Info("Using SFTP client");
            return new OrigenceSftpClient(settings);
        }
        else
        {
            Logger.Info("Using FTP client");
            return new OrigenceFtpClient(settings);
        }
    }
}