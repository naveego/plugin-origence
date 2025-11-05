using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentFTP;
using PluginOrigence.Helper;

public class OrigenceFtpClient : IPluginClient
{
    private Settings Settings { get; set; }
    private readonly FtpClient _client;

    public OrigenceFtpClient(Settings settings)
    {
        Settings = settings;
        _client = new FtpClient(Settings.Hostname, Settings.Username, Settings.Password, Settings.Port);
    }

    public bool Connect()
    {
        try
        {
            if (!IsConnected())
            {
                _client.Connect();
            }
            return true;

        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Disconnect()
    {
        try
        {
            if (IsConnected())
            {
                _client.Disconnect();
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool IsConnected()
    {
        return _client.IsConnected;
    }

    public async IAsyncEnumerable<XDocument> GetDocuments()
    {
        Connect();

        var files = _client.GetListing(Settings.RootPath);

        foreach (var file in files)
        {
            if (file.Type == FtpObjectType.File && file.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = new MemoryStream();
                XDocument xDoc = null;
                
                try
                {
                    _client.DownloadStream(stream, file.FullName);
                    stream.Position = 0;
                    xDoc = await Task.Run(() => XDocument.Load(stream));
                }
                catch (Exception)
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
