using Naveego.Sdk.Plugins;
using PluginOrigence.Helper;

namespace PluginOrigence.Helper
{
    public class ServerStatus
    {
        public ConfigureRequest Config { get; set; }
        public Settings Settings { get; set; }
        public bool Connected { get; set; }
    }
}