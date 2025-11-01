using Google.Protobuf;
using Renci.SshNet;
using PluginOrigence.Helper;
using System.Threading.Tasks;
using System;

public class OrigenceSftpClient : IPluginClient
{
    private Settings _settings { get; set; }
    private SftpClient _client;

    public OrigenceSftpClient(Settings settings)
    {
        _settings = settings;

        if (!string.IsNullOrWhiteSpace(_settings.Password))
        {
            _client = new SftpClient(_settings.Hostname, _settings.Port, _settings.Username, _settings.Password);
        }

        if (!string.IsNullOrWhiteSpace(_settings.SshKey))
        {

            _client = new SftpClient(_settings.Hostname, _settings.Port, _settings.Username,  new PrivateKeyFile(_settings.SshKey));
        }

        _client!.KeepAliveInterval = TimeSpan.FromMinutes(60);

    }

    public async Task<bool> Connect()
    {
        try
        {
            _client?.Connect();
            return true;

        }
        catch (Exception)
        {
            // Handle connection errors
            return false;
        }
    }

    public async Task<bool> Disconnect()
    {
        try
        {
            _client?.Disconnect();
            return true;
        }
        catch (Exception)
        {
            // Handle disconnection errors
            return false;
        }
    }

    public bool IsConnected()
    {
        return _client?.IsConnected ?? false;
    }
}