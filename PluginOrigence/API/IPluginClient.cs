using System.Collections.Generic;
using System.Xml.Linq;

public interface IPluginClient
{
    bool Connect();
    IAsyncEnumerable<XElement> GetData(string elementTag, int sampleSize = 0);
    bool Disconnect();
    bool IsConnected();
}
