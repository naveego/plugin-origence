using System;
using PluginOrigence.Helper;

public class ClientFactory : IClientFactory
{
    public IPluginClient GetClient(Settings settings)
    {
        if (settings.HostType == "ftp")
        {
            return new OrigenceFtpClient(settings);
        } 
        else if (settings.HostType == "sftp")
        {
            return new OrigenceSftpClient(settings);
        } 
        else if (settings.HostType == "local")
        {
            return new OrigenceLfsClient(settings);
        }
        else
        {
            throw new Exception("Invalid HostType");
        }
        
    }
}