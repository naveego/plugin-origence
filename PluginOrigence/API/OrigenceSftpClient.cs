using Renci.SshNet;
using PluginOrigence.Helper;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public class OrigenceSftpClient : IPluginClient
{
    private Settings Settings { get; set; }
    private readonly SftpClient _client;

    public OrigenceSftpClient(Settings settings)
    {
        Settings = settings;

        if (!string.IsNullOrWhiteSpace(Settings.SshKey))
        {
            var secretKey = Settings.SshKey;
            MemoryStream keystream;
            try
            {
                var start = Regex.Match(secretKey, "^-----BEGIN (?:RSA |OPENSSH |ED25519 )?PRIVATE KEY-----").Groups[0].Value;
                var end = Regex.Match(secretKey, "-----END (?:RSA |OPENSSH |ED25519 )?PRIVATE KEY-----$").Groups[0].Value;

                var endOfStartIndex = secretKey.IndexOf(start) + start.Length;
                var startOfEndIndex = secretKey.LastIndexOf(end);

                var center = secretKey.Substring(endOfStartIndex, startOfEndIndex - endOfStartIndex).Replace(" ", "\r\n");
                var formattedSshKey = start + center + end;

                keystream = new MemoryStream(Encoding.ASCII.GetBytes(formattedSshKey));
            }
            catch (Exception)
            {
                throw new Exception("Invalid SSH key format. Please provide a valid private key.");
            }
            
            _client = new SftpClient(Settings.Hostname, Settings.Port, Settings.Username, new PrivateKeyFile(keystream));
        }
        else if (!string.IsNullOrWhiteSpace(Settings.Password))
        {
            _client = new SftpClient(Settings.Hostname, Settings.Port, Settings.Username, Settings.Password);
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
            return _client.Exists(Settings.RootPath);
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

    public async IAsyncEnumerable<XDocument> GetDocuments()
    {
        Connect();

        var files = _client?.ListDirectory(Settings.RootPath);

        foreach (var file in files)
        {
            if (file.IsRegularFile && file.Name.ToLower().EndsWith(".xml"))
            {
                using var stream = _client.OpenRead(file.FullName);
                XDocument xDoc = null;

                try
                {
                    xDoc = await Task.Run(() => XDocument.Load(stream));
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