using Renci.SshNet;
using PluginOrigence.Helper;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;

public class OrigenceSftpClient : IPluginClient
{
    private Settings Settings { get; set; }
    private readonly SftpClient _client;

    public OrigenceSftpClient(Settings settings)
    {
        Settings = settings;

        if (!string.IsNullOrWhiteSpace(Settings.Password))
        {
            _client = new SftpClient(Settings.Hostname, Settings.Port, Settings.Username, Settings.Password);
        }

        if (!string.IsNullOrWhiteSpace(Settings.SshKey))
        {

            _client = new SftpClient(Settings.Hostname, Settings.Port, Settings.Username, new PrivateKeyFile(Settings.SshKey));
        }

        _client!.KeepAliveInterval = TimeSpan.FromMinutes(60);

    }

    public bool Connect()
    {
        try
        {
            if (!IsConnected())
            {
                _client?.Connect();
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
            _client?.Disconnect();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool IsConnected()
    {
        return _client?.IsConnected ?? false;
    }

    public async IAsyncEnumerable<XElement> GetData(string elementTag, int sampleSize)
    {
        Connect();

        var files = _client?.ListDirectory(Settings.RootPath);
        var count = 0;

        foreach (var file in files)
        {
            if (file.IsRegularFile && file.Name.ToLower().EndsWith(".xml"))
            {
                using var stream = _client.OpenRead(file.FullName);
                var xDoc = await Task.Run(() => XDocument.Load(stream));

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