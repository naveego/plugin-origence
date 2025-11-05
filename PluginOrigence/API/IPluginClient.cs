using System.Collections.Generic;
using System.Xml.Linq;

public interface IPluginClient
{
    bool Connect();
    IAsyncEnumerable<XDocument> GetDocuments();
    bool Disconnect();
    bool IsConnected();
}
