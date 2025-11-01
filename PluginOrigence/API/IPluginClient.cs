using System.Threading.Tasks;
using PluginOrigence.Helper;

public interface IPluginClient
{
    Task<bool> Connect();
    Task<bool> Disconnect();
    bool IsConnected();
}
