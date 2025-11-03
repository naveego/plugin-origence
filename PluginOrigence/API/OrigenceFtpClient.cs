using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public async IAsyncEnumerable<XElement> GetData(string elementTag, int sampleSize)
    {
        Connect();

        var files = _client.GetListing(Settings.RootPath);
        var count = 0;

        foreach (var file in files)
        {
            if (file.Type == FtpObjectType.File && file.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = new MemoryStream();
                
                _client.DownloadStream(stream, file.FullName);
                stream.Position = 0;
                var xDoc = XDocument.Load(stream);
                
                var ns = xDoc?.Root?.Name.Namespace ?? "";
                xDoc.Descendants(ns + "Raw").Remove();
                var elementsWithNs = xDoc?.Descendants(ns + elementTag).ToList();

                foreach (var element in elementsWithNs)
                {
                    if (sampleSize > 0 && count >= sampleSize)
                    {
                        yield break;
                    }
                    count++;
                    yield return element;
                }
            }
        }

    }
}
