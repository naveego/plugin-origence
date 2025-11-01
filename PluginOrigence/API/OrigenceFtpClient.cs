using System.Threading.Tasks;
using PluginOrigence.Helper;

public class OrigenceFtpClient : IPluginClient
{
    public Settings Settings { get; set; }

    public Task<bool> Connect()
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> Disconnect()
    {
        throw new System.NotImplementedException();
    }

    public bool IsConnected()
    {
        throw new System.NotImplementedException();
    }
}
