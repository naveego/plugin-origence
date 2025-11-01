using PluginOrigence.Helper;

public interface IClientFactory
{
    IPluginClient GetClient(Settings settings);
}
