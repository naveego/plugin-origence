using PluginOrigence.Helper;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrigenceLfsClient : IPluginClient
{
    private Settings Settings { get; set; }

    public OrigenceLfsClient(Settings settings)
    {
        Settings = settings;
    }

    public bool Connect()
    {
        return Directory.Exists(Settings.RootPath);
    }

    public bool Disconnect()
    {
        return true;
    }

    public bool IsConnected()
    {
        return Directory.Exists(Settings.RootPath);
    }

    public async IAsyncEnumerable<XDocument> GetDocuments()
    {
        if (IsConnected())
        {
            var files = Directory.GetFiles(Settings.RootPath, "*.xml");

            foreach (var file in files)
            {
                XDocument xDoc = null;
                try
                {
                    xDoc = await Task.Run(() => XDocument.Load(file));
                }
                catch
                {
                    // ignored
                }

                if (xDoc != null)
                {
                    yield return xDoc;
                }
            }
        }
    }
}
